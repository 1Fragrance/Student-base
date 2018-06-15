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
    public partial class MainWindow : Window
    {
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
        
        public GroupEnum groups;
        public StudentEnum students;
        string readConfig;
        KeyGesture hotkey;

        AppSettingsReader reader;

        public MainWindow()
        {
            InitializeComponent();
            
            groups = new GroupEnum();
            students = new StudentEnum();

            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);


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
            Environment.Exit(1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StudentsWindow studentsWindow = new StudentsWindow(this, groups, students);
            studentsWindow.Show();
            Hide();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GroupsWindow groupsWindow = new GroupsWindow(this, groups, students);
            groupsWindow.Show();
            Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
            {
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
            }
            Environment.Exit(1);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
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


        private void Button_Click_4(object sender, RoutedEventArgs e)
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

            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
            {
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
            }

            MessageBox.Show("File loaded", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Plugins plugWindow = new Plugins(this);
            plugWindow.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            groups.groupList.Clear();
            students.studentlist.Clear();
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

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
            {
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
            }
            Environment.Exit(1);
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Plugins pluginsWindow = new Plugins(this);
            pluginsWindow.JustRun();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Plugins pluginsWindow = new Plugins(this);
            pluginsWindow.ShowDialog();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow setWindow = new SettingsWindow(this);
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
                    if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
                    {
                        Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
                    }
                    Environment.Exit(1);
                }, o => true), hotkey);
                InputBindings.Add(closeWindowHotkey);

                readConfig = (string)reader.GetValue("pluginsMenu", typeof(string));
                openPlMenuButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding pluginsMenuHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(this);
                    pluginsWindow.ShowDialog();
                }, o => true), hotkey);
                InputBindings.Add(pluginsMenuHotkey);

                readConfig = (string)reader.GetValue("runPlugins", typeof(string));
                runButton.InputGestureText = readConfig;
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString(readConfig);
                KeyBinding runPluginsHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(this);
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
                MessageBox.Show("Config file not loaded, loaded standart settings", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Buffer"))
                    {
                        Directory.Delete(Directory.GetCurrentDirectory() + @"\Buffer", true);
                    }
                    Environment.Exit(1);
                }, o => true), hotkey);
                InputBindings.Add(closeWindowHotkey);
                openPlMenuButton.InputGestureText = "F2";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("F2");
                KeyBinding pluginsMenuHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(this);
                    pluginsWindow.ShowDialog();
                }, o => true), hotkey);
                InputBindings.Add(pluginsMenuHotkey);
                runButton.InputGestureText = "F1";
                hotkey = (KeyGesture)new KeyGestureConverter().ConvertFromString("F1");
                KeyBinding runPluginsHotkey = new KeyBinding(new RelayCommand(o =>
                {
                    Plugins pluginsWindow = new Plugins(this);
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



