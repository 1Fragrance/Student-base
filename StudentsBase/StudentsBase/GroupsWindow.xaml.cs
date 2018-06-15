using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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

namespace StudentsBase
{

    public partial class GroupsWindow : Window
    {
        string readConfig;
        KeyGesture hotkey;

        AppSettingsReader reader = new AppSettingsReader();
        GroupEnum groups;
        StudentEnum students;
        MainWindow mainWindow;
        bool check;
        Group subject = new Group();

        public GroupsWindow(MainWindow _mainWindow, GroupEnum _groups, StudentEnum _students)
        {
            InitializeComponent();

            groups = _groups;
            students = _students;
            mainWindow = _mainWindow;
            check = false;

            List.ItemsSource = groups.groupList;
            List.Items.Refresh();
            SubjectGrid.ItemsSource = subject.teachers;

            HeadCombo.ItemsSource = students.studentlist;
            HeadCombo.DisplayMemberPath = "Surname" ;

            newButtonImage.Source = BitmapToImageSource(StudentsBaseResources.newIcon);
            openButtonImage.Source = BitmapToImageSource(StudentsBaseResources.openIcon);
            saveButtonImage.Source = BitmapToImageSource(StudentsBaseResources.saveIcon);
            menuButtonImage.Source = BitmapToImageSource(StudentsBaseResources.menuIcon);
            runButtonImage.Source = BitmapToImageSource(StudentsBaseResources.runIcon);
            settingsButtonImage.Source = BitmapToImageSource(StudentsBaseResources.settingsIcon);

            refreshMenu();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!check)
            {
                mainWindow.Show();
                Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < groups.groupList.Count(); i++)
            {
                if (NumberBox.Text == Convert.ToString(groups.groupList[i].Number))
                {
                    MessageBox.Show("Sorry, you can't create 2 similar groups", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    NumberBox.Clear();
                    return;
                }
            }

            if (String.IsNullOrWhiteSpace(NumberBox.Text))
            {
                MessageBox.Show("Please, input the number of group", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                NumberBox.Clear();
                return;
            }

            if (Convert.ToInt32(NumberBox.Text) > 999999 )
            {
                MessageBox.Show("Please, input correct number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                NumberBox.Clear();
                return;
            }
            

            if(HeadCombo.SelectedItem != null)
            {
                var rez = (Student)HeadCombo.SelectedItem; 
                if(rez.isHead == "+")
                {
                    MessageBox.Show("This student is already a head", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    HeadCombo.SelectedItem = null;
                    return;
                }
            }

            Group new_group = new Group();

            new_group.Number = Convert.ToInt32(NumberBox.Text);

            if (CheckExisting.IsChecked == true)
                new_group.Actual = "+";
            else
                new_group.Actual = "-";

            if (HeadCombo.SelectedItem == null)
            {
                new_group.head = null;
                new_group.headName = "-";
            }
            else
            {
                new_group.head = (Student)HeadCombo.SelectedItem;
                new_group.headName = new_group.head.Surname + " " + new_group.head.Name;

                var rez = new_group.head;
                rez.isHead = "+";
                rez.group_number = Convert.ToString(new_group.Number);
                rez.groupHeading = rez.group_number;
                rez.group = new_group;
            }
            new_group.teachers = new List<Group.struct_teachers>(subject.teachers);

            groups.groupList.Add(new_group);

            List.Items.Refresh();
            subject.teachers.Clear();
            SubjectGrid.ItemsSource = subject.teachers;
            SubjectGrid.Items.Refresh();
            NumberBox.Clear();
            HeadCombo.SelectedIndex = -1;

            CheckExisting.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StudentsWindow studentsWindow = new StudentsWindow(mainWindow,groups,students);
            studentsWindow.Show();
            check = true;
            Close();
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List.SelectedIndex != -1)
            {
                int selectedIndex = List.SelectedIndex;

                NumberBox.Text = Convert.ToString(groups.groupList[selectedIndex].Number);

                if (groups.groupList[selectedIndex].Actual == "+")
                    CheckExisting.IsChecked = true;
                else
                    CheckExisting.IsChecked = false;

                if (groups.groupList[selectedIndex].head != null)
                    HeadCombo.SelectedItem = groups.groupList[selectedIndex].head;
                else
                    HeadCombo.SelectedItem = null;

                subject.teachers = new List<Group.struct_teachers>(groups.groupList[selectedIndex].teachers);
                SubjectGrid.ItemsSource = subject.teachers;
                SubjectGrid.Items.Refresh();

                ChangeButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = List.SelectedIndex;

            if(groups.groupList[selectedIndex].head != null)
            {
                var rez = groups.groupList[selectedIndex].head;
                rez.isHead = "-";                
                rez.groupHeading = "-";
            }

            for (int i = 0; i < students.studentlist.Count(); i++)
            {
                if (students.studentlist[i].group == groups.groupList[selectedIndex])
                {
                    students.studentlist[i].group = null;
                    students.studentlist[i].group_number = "-";
                }
                if (students.studentlist[i].oldGroup == groups.groupList[selectedIndex])
                {
                    students.studentlist[i].oldGroup = null;
                    students.studentlist[i].old_group_number = "-";
                }
            }

            groups.groupList.Remove(groups.groupList[selectedIndex]);

            List.Items.Refresh();
            NumberBox.Clear();
            HeadCombo.SelectedIndex = -1;
            subject.teachers.Clear();
            SubjectGrid.ItemsSource = subject.teachers;
            SubjectGrid.Items.Refresh();

            CheckExisting.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false; 
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < groups.groupList.Count(); i++)
            {
                if (groups.groupList[List.SelectedIndex].Number == Convert.ToInt32(NumberBox.Text))
                    continue;
                if (groups.groupList[i].Number == Convert.ToInt32(NumberBox.Text))
                {
                    MessageBox.Show("Please, fill the number of group", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    NumberBox.Clear();
                    return;
                }
            }

            if (String.IsNullOrWhiteSpace(NumberBox.Text))
            {
                MessageBox.Show("Please, input the number of group", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                NumberBox.Clear();
                return;
            }

            if (HeadCombo.SelectedItem != null)
            {
                var rez = (Student)HeadCombo.SelectedItem;
                if (rez.isHead == "+")
                {
                    MessageBox.Show("This student is already a head", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    HeadCombo.SelectedItem = null;
                    return;
                }
            }

            var selectedIndex = List.SelectedIndex;

            for (int i = 0; i < students.studentlist.Count(); i++)
            {
                if (students.studentlist[i].group_number == Convert.ToString(groups.groupList[selectedIndex].Number))
                    students.studentlist[i].group_number = NumberBox.Text;
            }
            groups.groupList[selectedIndex].Number = Convert.ToInt32(NumberBox.Text);
            
            if (HeadCombo.SelectedItem == null)
            {
                groups.groupList[selectedIndex].head = null;
                groups.groupList[selectedIndex].headName = "-";
            }
            else
            {
                var rez1 = groups.groupList[selectedIndex].head;
                rez1.groupHeading = "-";
                rez1.isHead = "-";

                groups.groupList[selectedIndex].head = (Student)HeadCombo.SelectedItem;
                groups.groupList[selectedIndex].headName = groups.groupList[selectedIndex].head.Surname + " " + groups.groupList[selectedIndex].head.Name;
                
                var rez = groups.groupList[selectedIndex].head;
                rez.isHead = "+";
                rez.group_number = Convert.ToString(groups.groupList[selectedIndex].Number);
                rez.groupHeading = rez.group_number;
                rez.group = groups.groupList[selectedIndex];
            }

            if (CheckExisting.IsChecked == false)
                groups.groupList[selectedIndex].Actual = "-";
            else
                groups.groupList[selectedIndex].Actual = "+";

            groups.groupList[selectedIndex].teachers = new List<Group.struct_teachers>(subject.teachers);

            List.Items.Refresh();
            NumberBox.Clear();            
            HeadCombo.SelectedIndex = -1;            
            subject.teachers.Clear();
            SubjectGrid.ItemsSource = subject.teachers;
            SubjectGrid.Items.Refresh();

            CheckExisting.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (List.SelectedItem != null)
            {
                var selectedItem = List.SelectedItem;
                GroupInfo groupsinfo = new GroupInfo(selectedItem, groups, students, List.SelectedIndex);
                groupsinfo.ShowDialog();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            newSubject groupsinfo = new newSubject(this, subject);
            groupsinfo.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var selectedIndex = SubjectGrid.SelectedIndex;
            subject.teachers.Remove(subject.teachers[selectedIndex]);
            SubjectGrid.Items.Refresh();
        }

        private void SubjectGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SubjectGrid.SelectedItem != null)
                DeleteSubject.IsEnabled = true;
        }

        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            List.Items.Refresh();
            NumberBox.Clear();
            HeadCombo.SelectedIndex = -1;
            subject.teachers.Clear();
            SubjectGrid.ItemsSource = subject.teachers;
            SubjectGrid.Items.Refresh();

            CheckExisting.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void List_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List.Items.Refresh();
            List.SelectedIndex = -1;
            NumberBox.Clear();            
            HeadCombo.SelectedIndex = -1;            
            subject.teachers.Clear();
            SubjectGrid.ItemsSource = subject.teachers;
            SubjectGrid.Items.Refresh();

            CheckExisting.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void SearchField_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(SearchField.Text))
            {
                List.ItemsSource = groups.groupList;
                List.Items.Refresh();
                return;
            }

            var searchTask = groups.groupList.Where(task => Convert.ToString(task.Number).Contains(SearchField.Text));
            List.ItemsSource = searchTask;
            List.Items.Refresh();
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            var sortList = groups.groupList.GroupBy(group => group.Actual);
            var bufferList = new List<Group>();

            foreach (IGrouping<string, Group> group in sortList)
                foreach (Group _group in group)
                    bufferList.Add(_group);

            groups.groupList = new List<Group>(bufferList);

            List.ItemsSource = groups.groupList;
            List.Items.Refresh();
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var sortList = groups.groupList.OrderBy(group => group.Number);
            groups.groupList = new List<Group>(sortList);

            List.ItemsSource = groups.groupList;
            List.Items.Refresh();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
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

            List.ItemsSource = groups.groupList;
            List.Items.Refresh();
            SubjectGrid.ItemsSource = subject.teachers;

            HeadCombo.ItemsSource = students.studentlist;
            HeadCombo.DisplayMemberPath = "Surname";
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

            List.ItemsSource = groups.groupList;
            List.Items.Refresh();
            SubjectGrid.ItemsSource = subject.teachers;

            List.SelectedIndex = -1;
            NumberBox.Clear();            
            HeadCombo.SelectedIndex = -1;            
            subject.teachers.Clear();
            SubjectGrid.ItemsSource = subject.teachers;
            SubjectGrid.Items.Refresh();

            CheckExisting.IsChecked = false;
            ChangeButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

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

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow setWindow = new SettingsWindow(mainWindow);
            setWindow.ShowDialog();
            refreshMenu();
        }

        private void refreshMenu()
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

