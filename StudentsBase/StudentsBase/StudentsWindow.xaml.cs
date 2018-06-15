using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using System.Windows.Interop;
using System.Configuration;

namespace StudentsBase
{    
    public partial class StudentsWindow : Window
    {
        string readConfig;
        KeyGesture hotkey;

        AppSettingsReader reader = new AppSettingsReader();
        MainWindow mainWindow;
        bool check;
        Student mark = new Student();
        GroupEnum groups;
        StudentEnum students;

        public StudentsWindow(MainWindow _mainWindow, GroupEnum _groups, StudentEnum _students)
        {
            InitializeComponent();


            newButtonImage.Source = BitmapToImageSource(StudentsBaseResources.newIcon);
            openButtonImage.Source = BitmapToImageSource(StudentsBaseResources.openIcon);
            saveButtonImage.Source = BitmapToImageSource(StudentsBaseResources.saveIcon);
            menuButtonImage.Source = BitmapToImageSource(StudentsBaseResources.menuIcon);
            runButtonImage.Source = BitmapToImageSource(StudentsBaseResources.runIcon);
            settingsButtonImage.Source = BitmapToImageSource(StudentsBaseResources.settingsIcon);

            refreshMenu();

            groups = _groups;
            students = _students;
            mainWindow = _mainWindow;
            check = false;
            
            List.ItemsSource = students.studentlist;
            List.Items.Refresh();
            List.SelectedIndex = -1;
           
            GroupCombo.ItemsSource = groups.groupList;
            GroupCombo.DisplayMemberPath = "Number";
            OldGroupCombo.ItemsSource = groups.groupList;
            OldGroupCombo.DisplayMemberPath = "Number";

            Table.ItemsSource = mark.marks;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            if (!check)
            {
                mainWindow.Show();
                Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "*.BMP *.JPG *.PNG *.JPEG | *.bmp; *.jpg; *.png; *.jpeg";

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Add new student
        {
            if(String.IsNullOrWhiteSpace(NameBox.Text) || String.IsNullOrWhiteSpace(SurnameBox.Text) || String.IsNullOrWhiteSpace(Middlename.Text) || String.IsNullOrWhiteSpace(YearBox.Text))
            {
                MessageBox.Show("Please, fill all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);                
                return;
            }

            if(NameBox.Text.Length > 16 || SurnameBox.Text.Length > 16 || Middlename.Text.Length > 16)
            {
                MessageBox.Show("Name, Surname and Middlename must not contain more than 16 letters ","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(Convert.ToInt32(YearBox.Text) > 2017 || Convert.ToInt32(YearBox.Text) < 1950)
            {
                MessageBox.Show("In the year selection field number should be between 1950 to 2017", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                YearBox.Clear();
                return;
            }

            if(GroupCombo.SelectedItem != null && GroupCombo.SelectedItem == OldGroupCombo.SelectedItem)
            {
                MessageBox.Show("Old group and current group must be different", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                GroupCombo.SelectedItem = null;
                OldGroupCombo.SelectedItem = null;
                return;
            }

            Student new_student = new Student();

            new_student.Name = NameBox.Text;
            new_student.Surname = SurnameBox.Text;
            new_student.Middlename = Middlename.Text;
            new_student.Year = Convert.ToInt32(YearBox.Text);

            if (GroupCombo.SelectedItem == null)
            {
                new_student.group_number = "-";
                new_student.isHead = "-";
                new_student.groupHeading = "-";
                new_student.group = null;
            }
            else
            {
                new_student.group_number = GroupCombo.Text;

                if (HeadCheck.IsChecked == true)
                {
                    var rez = (Group)GroupCombo.SelectedItem;
                    if (rez.head == null)
                    {
                        new_student.isHead = "+";

                        rez.headName = new_student.Surname + " " + new_student.Name;
                        rez.head = new_student;
                    }
                    else
                    {
                        MessageBox.Show("This group already has a head. This student was not appointed head", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        new_student.isHead = "-";
                    }
                }
                else
                    new_student.isHead = "-";

                new_student.group = (Group)GroupCombo.SelectedItem;
            }

            if (new_student.isHead == "+")
                new_student.groupHeading = new_student.group_number;
            else
                new_student.groupHeading = "-";

            if (OldGroupCombo.SelectedItem == null)
                new_student.old_group_number = "-";
            else
                new_student.old_group_number = OldGroupCombo.Text;

            new_student.oldGroup = (Group)OldGroupCombo.SelectedItem;

            if (FileNameTextBox.Text != "")
            {
                BitmapImage StudentPhoto = new BitmapImage();
                StudentPhoto.BeginInit();
                StudentPhoto.UriSource = new Uri(FileNameTextBox.Text, UriKind.Absolute);
                StudentPhoto.CacheOption = BitmapCacheOption.OnLoad;
                StudentPhoto.EndInit();
                new_student.studentPhoto = StudentPhoto;
                new_student.photoPath = Convert.ToString(new_student.studentPhoto.UriSource);
            }

            new_student.marks = new List<Student.struct_marks>(mark.marks);
            students.studentlist.Add(new_student);

            List.Items.Refresh();
            mark.marks.Clear();
            Table.ItemsSource = mark.marks;
            Table.Items.Refresh();

            NameBox.Clear();
            SurnameBox.Clear();
            Middlename.Clear();
            YearBox.Clear();

            GroupCombo.SelectedIndex = -1;
            OldGroupCombo.SelectedIndex = -1;
            HeadCheck.IsChecked = false;
            FileNameTextBox.Text = "";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GroupsWindow groupsWindow = new GroupsWindow(mainWindow, groups, students);
            groupsWindow.Show();
            check = true;
            Close();
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List.SelectedIndex != -1)
            {
                int selectedIndex = List.SelectedIndex;

                NameBox.Text = Convert.ToString(students.studentlist[selectedIndex].Name);
                SurnameBox.Text = Convert.ToString(students.studentlist[selectedIndex].Surname);
                Middlename.Text = Convert.ToString(students.studentlist[selectedIndex].Middlename);
                YearBox.Text = Convert.ToString(students.studentlist[selectedIndex].Year);

                if (students.studentlist[selectedIndex].isHead == "+")
                    HeadCheck.IsChecked = true;
                else
                    HeadCheck.IsChecked = false;

                if (students.studentlist[selectedIndex].group != null)
                    GroupCombo.SelectedItem = students.studentlist[selectedIndex].group;
                else
                    GroupCombo.SelectedItem = null;

                if (students.studentlist[selectedIndex].oldGroup != null)
                    OldGroupCombo.SelectedItem = students.studentlist[selectedIndex].oldGroup;
                else
                    OldGroupCombo.SelectedItem = null;

                if (students.studentlist[selectedIndex].studentPhoto != null)
                    FileNameTextBox.Text = Convert.ToString(students.studentlist[selectedIndex].studentPhoto.UriSource);
                else
                    FileNameTextBox.Text = "";

                mark.marks = new List<Student.struct_marks>(students.studentlist[selectedIndex].marks);
                Table.ItemsSource = mark.marks;
                Table.Items.Refresh();

                AddButton.IsEnabled = false;
                ChangeButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            } 
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // Change information
        {
            if (String.IsNullOrWhiteSpace(NameBox.Text) || String.IsNullOrWhiteSpace(SurnameBox.Text) || String.IsNullOrWhiteSpace(Middlename.Text) || String.IsNullOrWhiteSpace(YearBox.Text))
            {
                MessageBox.Show("Please, fill all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NameBox.Text.Length > 16 || SurnameBox.Text.Length > 16 || Middlename.Text.Length > 16)
            {
                MessageBox.Show("Name, Surname and Middlename must not contain more than 16 letters ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Convert.ToInt32(YearBox.Text) > 2017 || Convert.ToInt32(YearBox.Text) < 1950)
            {
                MessageBox.Show("In the year selection field number should be between 1950 to 2017", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                YearBox.Clear();
                return;
            }

            if (GroupCombo.SelectedItem != null && GroupCombo.SelectedItem == OldGroupCombo.SelectedItem)
            {
                MessageBox.Show("Old group and current group must be different", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                GroupCombo.SelectedItem = null;
                OldGroupCombo.SelectedItem = null;
                return;
            }

            var selectedIndex = List.SelectedIndex;

            students.studentlist[selectedIndex].Name = NameBox.Text;
            students.studentlist[selectedIndex].Surname = SurnameBox.Text;
            students.studentlist[selectedIndex].Middlename = Middlename.Text;
            students.studentlist[selectedIndex].Year = Convert.ToInt32(YearBox.Text);

            if (GroupCombo.SelectedItem == null)
            {
                students.studentlist[selectedIndex].group_number = "-";
                students.studentlist[selectedIndex].groupHeading = "-";
                students.studentlist[selectedIndex].isHead = "-";
                students.studentlist[selectedIndex].group = null;
            }
            else
            {
                students.studentlist[selectedIndex].group_number = GroupCombo.Text;
                var rez = (Group)GroupCombo.SelectedItem;

                if (HeadCheck.IsChecked == true)
                {
                    if (students.studentlist[selectedIndex].groupHeading != Convert.ToString(rez.Number))
                    {
                        if (rez != students.studentlist[selectedIndex].group)
                            if (students.studentlist[selectedIndex].isHead == "+")
                            {
                                var rez1 = students.studentlist[selectedIndex].group;
                                rez1.headName = "-";
                                rez1.head = null;
                            }

                        if (rez.head == null)
                        {
                            students.studentlist[selectedIndex].isHead = "+";
                            students.studentlist[selectedIndex].group_number = Convert.ToString(rez.Number);
                            students.studentlist[selectedIndex].groupHeading = students.studentlist[selectedIndex].group_number;
                            students.studentlist[selectedIndex].group = rez;

                            rez.headName = students.studentlist[selectedIndex].Surname + " " + students.studentlist[selectedIndex].Name;
                            rez.head = students.studentlist[selectedIndex];
                        }
                        else
                        {
                            students.studentlist[selectedIndex].group_number = Convert.ToString(rez.Number);
                            students.studentlist[selectedIndex].isHead = "-";
                            students.studentlist[selectedIndex].groupHeading = "-";
                            students.studentlist[selectedIndex].group = rez;
                            MessageBox.Show("This group already has a head. This student was not appointed head", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        rez.headName = students.studentlist[selectedIndex].Surname + " " + students.studentlist[selectedIndex].Name;
                    }
                }
                else
                {
                    if(rez.head == students.studentlist[selectedIndex])
                    {
                        rez.head = null;
                        rez.headName = "-";
                    }
                    students.studentlist[selectedIndex].isHead = "-";
                    students.studentlist[selectedIndex].groupHeading = "-";                    
                }
                students.studentlist[selectedIndex].group = (Group)GroupCombo.SelectedItem;
            }

            if (students.studentlist[selectedIndex].isHead == "+")
                students.studentlist[selectedIndex].groupHeading = students.studentlist[selectedIndex].group_number;
            else
                students.studentlist[selectedIndex].groupHeading = "-";

            if (OldGroupCombo.SelectedItem == null)
                students.studentlist[selectedIndex].old_group_number = "-";
            else
                students.studentlist[selectedIndex].old_group_number = OldGroupCombo.Text;

            students.studentlist[selectedIndex].oldGroup = (Group)OldGroupCombo.SelectedItem;

            if (students.studentlist[selectedIndex].studentPhoto != null)
            {
                if ("file:///" + FileNameTextBox.Text != Convert.ToString(students.studentlist[selectedIndex].studentPhoto.UriSource))
                {
                    if (FileNameTextBox.Text != "")
                    {
                        BitmapImage StudentPhoto = new BitmapImage();
                        StudentPhoto.BeginInit();
                        StudentPhoto.UriSource = new Uri(FileNameTextBox.Text, UriKind.Absolute);
                        StudentPhoto.CacheOption = BitmapCacheOption.OnLoad;
                        StudentPhoto.EndInit();
                        students.studentlist[selectedIndex].studentPhoto = StudentPhoto;
                        students.studentlist[selectedIndex].photoPath = Convert.ToString(students.studentlist[selectedIndex].studentPhoto.UriSource);
                    }
                }
            }

            students.studentlist[selectedIndex].marks = new List<Student.struct_marks>(mark.marks);
            List.Items.Refresh();

            NameBox.Clear();
            SurnameBox.Clear();
            Middlename.Clear();
            YearBox.Clear();
            FileNameTextBox.Clear();
            GroupCombo.SelectedIndex = -1;
            OldGroupCombo.SelectedIndex = -1;

            mark.marks.Clear();
            Table.ItemsSource = mark.marks;
            Table.Items.Refresh();

            AddButton.IsEnabled = true;
            HeadCheck.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) // Delete selected student
        {
            var selectedIndex = List.SelectedIndex;

            if(students.studentlist[selectedIndex].isHead == "+")
            {
                var rez = students.studentlist[selectedIndex].group;
                rez.head = null;
                rez.headName = "-";
            }
            students.studentlist.Remove(students.studentlist[selectedIndex]);

            NameBox.Clear();
            SurnameBox.Clear();
            Middlename.Clear();
            YearBox.Clear();
            FileNameTextBox.Clear();
            GroupCombo.SelectedIndex = -1;
            OldGroupCombo.SelectedIndex = -1;

            List.Items.Refresh();

            mark.marks.Clear();
            Table.ItemsSource = mark.marks;
            Table.Items.Refresh();

            AddButton.IsEnabled = true;
            HeadCheck.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;            
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (List.SelectedItem != null)
            {
                var selectedItem = List.SelectedItem;
                StudentInfo studentsinfo = new StudentInfo(selectedItem, groups, students,List.SelectedIndex);
                studentsinfo.ShowDialog();
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            newMark newMark = new newMark(this, mark);
            newMark.ShowDialog();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            NameBox.Clear();
            SurnameBox.Clear();
            Middlename.Clear();
            YearBox.Clear();
            GroupCombo.SelectedIndex = -1;
            OldGroupCombo.SelectedIndex = -1;
            FileNameTextBox.Clear();

            mark.marks.Clear();
            Table.ItemsSource = mark.marks;
            Table.Items.Refresh();

            AddButton.IsEnabled = true;
            HeadCheck.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Table.SelectedItem != null)
                Delete_Mark.IsEnabled = true;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (Table.SelectedItem != null)
            {
                var selectedIndex = Table.SelectedIndex;
                mark.marks.Remove(mark.marks[selectedIndex]);

                Table.Items.Refresh();
                Delete_Mark.IsEnabled = false;
            }
        }

        private void List_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NameBox.Clear();
            SurnameBox.Clear();
            Middlename.Clear();
            YearBox.Clear();
            GroupCombo.SelectedIndex = -1;
            OldGroupCombo.SelectedIndex = -1;
            FileNameTextBox.Clear();

            mark.marks.Clear();
            Table.ItemsSource = mark.marks;
            Table.Items.Refresh();

            AddButton.IsEnabled = true;
            HeadCheck.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            List.SelectedIndex = -1;
        }

        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {  
            e.Handled = "0123456789".IndexOf(e.Text) > 0 ;
        }

        private void SurnameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) > 0;
        }

        private void Middlename_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) > 0;
        }

        private void YearBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;          
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var sortList = students.studentlist.AsParallel().OrderBy(student => student.Surname);
            students.studentlist = new List<Student>(sortList);

            List.ItemsSource = students.studentlist;
            List.Items.Refresh();
        }

        private void SearchField_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(SearchField.Text))
            {
                List.ItemsSource = students.studentlist;
                List.Items.Refresh();
                return;
            }
            var searchTask = students.studentlist.Where(task => task.Surname.Contains(SearchField.Text)).AsParallel();
            List.ItemsSource = searchTask;
            List.Items.Refresh();
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            var sortList = students.studentlist.AsParallel().GroupBy(student => student.group_number);
            var bufferList = new List<Student>();

            foreach (IGrouping<string, Student> group in sortList)
                foreach (Student student in group)
                    bufferList.Add(student);

            students.studentlist = new List<Student>(bufferList);

            List.ItemsSource = students.studentlist;
            List.Items.Refresh();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!check)
                {
                    mainWindow.Show();
                    Close();
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            groups.groupList.Clear();
            students.studentlist.Clear();
            List.ItemsSource = students.studentlist;
            List.Items.Refresh();
            List.SelectedIndex = -1;

            GroupCombo.ItemsSource = groups.groupList;
            GroupCombo.DisplayMemberPath = "Number";
            OldGroupCombo.ItemsSource = groups.groupList;
            OldGroupCombo.DisplayMemberPath = "Number";

            Table.ItemsSource = mark.marks;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            GroupEnum new_groups = new GroupEnum();
            StudentEnum new_students = new StudentEnum();


            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".zip";
            dialog.Filter = "zip|*.zip";

            string selectedArchive = "";
            bool? result = dialog.ShowDialog();
            if (result == true)
                selectedArchive = dialog.FileName;
            else
                return;
            try
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer") && selectedArchive != "")
                    ZipFile.ExtractToDirectory(selectedArchive, Directory.GetCurrentDirectory());
                else
                   return;
            }
            catch (Exception)
            {
                MessageBox.Show("Please, select another folder ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FileStream connector = new FileStream(Directory.GetCurrentDirectory() + @"\Buffer\Temp.txt", FileMode.Open); ;
            StreamReader reader = new StreamReader(connector);

            int groupsNumber = Convert.ToInt32(reader.ReadLine());
            reader.ReadLine();
            for (int i = 0; i < groupsNumber; i++)
            {
                reader.ReadLine();
                Group new_group = new Group();

                new_group.Number = Convert.ToInt32(reader.ReadLine());
                new_group.Actual = reader.ReadLine();
                new_group.headName = reader.ReadLine();
                int teachersNumber = Convert.ToInt32(reader.ReadLine());

                Group subject = new Group();
                if (teachersNumber != 0)
                {
                    reader.ReadLine();
                    for (int j = 0; j < teachersNumber; j++)
                    {
                        Group.struct_teachers new_teacher = new Group.struct_teachers();
                        new_teacher.Name = reader.ReadLine();
                        new_teacher.Subject = reader.ReadLine();
                        subject.teachers.Add(new_teacher);
                    }
                    reader.ReadLine();
                    new_group.teachers = new List<Group.struct_teachers>(subject.teachers);
                }
                else
                    reader.ReadLine();

                reader.ReadLine();
                new_groups.groupList.Add(new_group);
            }
            reader.ReadLine();

            int studentsNumber = Convert.ToInt32(reader.ReadLine());
            reader.ReadLine();
            for (int i = 0; i < studentsNumber; i++)
            {
                reader.ReadLine();
                Student new_student = new Student();

                new_student.Name = reader.ReadLine();
                new_student.Surname = reader.ReadLine();
                new_student.Middlename = reader.ReadLine();
                new_student.Year = Convert.ToInt32(reader.ReadLine());
                new_student.isHead = reader.ReadLine();
                new_student.groupHeading = reader.ReadLine();
                new_student.group_number = reader.ReadLine();
                new_student.old_group_number = reader.ReadLine();

                new_student.photoPath = reader.ReadLine();
                if (new_student.photoPath != "")
                {
                    BitmapImage StudentPhoto = new BitmapImage();
                    StudentPhoto.BeginInit();
                    StudentPhoto.UriSource = new Uri(new_student.photoPath, UriKind.Absolute);
                    StudentPhoto.CacheOption = BitmapCacheOption.OnLoad;
                    StudentPhoto.EndInit();
                    new_student.studentPhoto = StudentPhoto;
                }

                int marksNumber = Convert.ToInt32(reader.ReadLine());
                Student mark = new Student();
                if (marksNumber != 0)
                {
                    reader.ReadLine();
                    for (int j = 0; j < marksNumber; j++)
                    {
                        Student.struct_marks new_mark = new Student.struct_marks();
                        new_mark.Mark = Convert.ToInt32(reader.ReadLine());
                        new_mark.Subject = reader.ReadLine();
                        mark.marks.Add(new_mark);
                    }
                    reader.ReadLine();
                    new_student.marks = new List<Student.struct_marks>(mark.marks);
                }
                else
                    reader.ReadLine();

                reader.ReadLine();
                new_students.studentlist.Add(new_student);
            }

            for (int i = 0; i < new_groups.groupList.Count(); i++)
            {
                for (int j = 0; j < new_students.studentlist.Count(); j++)
                {
                    if (new_students.studentlist[j].group_number == Convert.ToString(new_groups.groupList[i].Number))
                    {
                        new_students.studentlist[j].group = new_groups.groupList[i];
                        if (new_students.studentlist[j].groupHeading == Convert.ToString(new_groups.groupList[i].Number))
                            new_groups.groupList[i].head = new_students.studentlist[j];
                    }
                    if (new_students.studentlist[j].old_group_number == Convert.ToString(new_groups.groupList[i].Number))
                    {
                        new_students.studentlist[j].oldGroup = new_groups.groupList[i];
                    }
                }
            }
            reader.Close();
            connector.Close();

            groups.groupList = new_groups.groupList;
            students.studentlist = new_students.studentlist;

            List.ItemsSource = students.studentlist;
            List.Items.Refresh();
            mark.marks.Clear();
            Table.ItemsSource = mark.marks;
            Table.Items.Refresh();

            NameBox.Clear();
            SurnameBox.Clear();
            Middlename.Clear();
            YearBox.Clear();

            GroupCombo.SelectedIndex = -1;
            OldGroupCombo.SelectedIndex = -1;
            HeadCheck.IsChecked = false;
            FileNameTextBox.Text = "";

            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
            {
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
            }

            MessageBox.Show("File loaded", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetCurrentDirectory() + @"\Buffer";
            Directory.CreateDirectory(path);    
            FileStream connector = File.Create(path + @"\Temp.txt");
            StreamWriter writer = new StreamWriter(connector);

            writer.WriteLine(groups.groupList.Count());
            writer.WriteLine("/Groups");
            for (int i = 0; i < groups.groupList.Count(); i++)
            {
                writer.WriteLine("/Group");
                writer.WriteLine(groups.groupList[i].Number);
                writer.WriteLine(groups.groupList[i].Actual);
                writer.WriteLine(groups.groupList[i].headName);
                writer.WriteLine(groups.groupList[i].teachers.Count());

                if (groups.groupList[i].teachers.Count() == 0)
                {
                    writer.WriteLine("\\Teachers");
                }
                else
                {
                    writer.WriteLine("/Teachers");
                    for (int j = 0; j < groups.groupList[i].teachers.Count(); j++)
                    {
                        writer.WriteLine(groups.groupList[i].teachers[j].Name);
                        writer.WriteLine(groups.groupList[i].teachers[j].Subject);
                    }
                    writer.WriteLine("\\Teachers");
                }
                writer.WriteLine("\\Group");
            }
            writer.WriteLine("\\Groups");

            writer.WriteLine(students.studentlist.Count());
            writer.WriteLine("/Students");
            for (int i = 0; i < students.studentlist.Count(); i++)
            {
                writer.WriteLine("/Student");
                writer.WriteLine(students.studentlist[i].Name);
                writer.WriteLine(students.studentlist[i].Surname);
                writer.WriteLine(students.studentlist[i].Middlename);
                writer.WriteLine(students.studentlist[i].Year);
                writer.WriteLine(students.studentlist[i].isHead);
                writer.WriteLine(students.studentlist[i].groupHeading);
                writer.WriteLine(students.studentlist[i].group_number);
                writer.WriteLine(students.studentlist[i].old_group_number);
                writer.WriteLine(students.studentlist[i].photoPath);
                writer.WriteLine(students.studentlist[i].marks.Count());
                if (students.studentlist[i].marks.Count() == 0)
                {
                    writer.WriteLine("\\Subjects");
                }
                else
                {
                    writer.WriteLine("/Subjects");
                    for (int j = 0; j < students.studentlist[i].marks.Count(); j++)
                    {
                        writer.WriteLine(students.studentlist[i].marks[j].Mark);
                        writer.WriteLine(students.studentlist[i].marks[j].Subject);
                    }
                    writer.WriteLine("\\Subjects");
                }
                writer.WriteLine("\\Student");
            }
            writer.WriteLine("\\Students");

            writer.Close();
            connector.Close();

            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            string Folder = "";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Folder = dialog.SelectedPath;
            }
            else
            {
                return;
            }
            if (Directory.Exists(path))
            {
                try
                {
                    ZipFile.CreateFromDirectory(path, Folder + ".zip", CompressionLevel.Fastest, true);
                    Directory.Delete(Folder, true);
                    MessageBox.Show("File saved", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
                    {
                        Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Please, select another folder ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Plugins pluginsWindow = new Plugins(mainWindow);
            pluginsWindow.JustRun();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Plugins pluginsWindow = new Plugins(mainWindow);
            pluginsWindow.ShowDialog();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
            {
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
            }
            Environment.Exit(1);
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow setWindow = new SettingsWindow(mainWindow);
            setWindow.ShowDialog();
            refreshMenu();
        }

        void refreshMenu()
        {
            reader = new AppSettingsReader();
            try
            {
                readConfig = (string)reader.GetValue("newFile", typeof(string));
                newButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding newFileHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    newButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(newFileHotkey);

                readConfig = (string)reader.GetValue("openFile", typeof(string));
                openButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding openFileHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    openButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(openFileHotkey);

                readConfig = (string)reader.GetValue("saveFile", typeof(string));
                saveButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding saveFileHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    saveButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(saveFileHotkey);

                readConfig = (string)reader.GetValue("closeWindow", typeof(string));
                exitButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding closeWindowHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    if (!check)
                    {
                        mainWindow.Show();
                        Close();
                    }
                }, o => true), hotkey);
                InputBindings.Add(closeWindowHotkey);

                readConfig = (string)reader.GetValue("pluginsMenu", typeof(string));
                openPlMenuButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding pluginsMenuHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(mainWindow);
                    pluginsWindow.ShowDialog();
                }, o => true), hotkey);
                InputBindings.Add(pluginsMenuHotkey);

                readConfig = (string)reader.GetValue("runPlugins", typeof(string));
                runButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding runPluginsHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(mainWindow);
                    pluginsWindow.JustRun();
                }, o => true), hotkey);
                InputBindings.Add(runPluginsHotkey);

                readConfig = (string)reader.GetValue("settings", typeof(string));
                settingsButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding settingsHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    settingsButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(settingsHotkey);
            }
            catch (Exception)
            {
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("Ctrl+N");
                newButton.InputGestureText = "Ctrl+N";
                KeyBinding newFileHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    newButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(newFileHotkey);
                openButton.InputGestureText = "Ctrl+L";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("Ctrl+L");
                KeyBinding openFileHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    openButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(openFileHotkey);
                saveButton.InputGestureText = "Ctrl+S";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("Ctrl+S");
                KeyBinding saveFileHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    saveButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(saveFileHotkey);
                exitButton.InputGestureText = "Esc";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("Esc");
                KeyBinding closeWindowHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    if (!check)
                    {
                        mainWindow.Show();
                        Close();
                    }
                }, o => true), hotkey);
                InputBindings.Add(closeWindowHotkey);
                openPlMenuButton.InputGestureText = "F2";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("F2");
                KeyBinding pluginsMenuHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(mainWindow);
                    pluginsWindow.ShowDialog();
                }, o => true), hotkey);
                InputBindings.Add(pluginsMenuHotkey);
                runButton.InputGestureText = "F1";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("F1");
                KeyBinding runPluginsHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(mainWindow);
                    pluginsWindow.JustRun();
                }, o => true), hotkey);
                InputBindings.Add(runPluginsHotkey);
                settingsButton.InputGestureText = "F9";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("F9");
                KeyBinding settingsHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    settingsButton.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }, o => true), hotkey);
                InputBindings.Add(settingsHotkey);
            }
        }
    }
}
