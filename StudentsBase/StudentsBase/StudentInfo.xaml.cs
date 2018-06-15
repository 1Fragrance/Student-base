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

namespace StudentsBase
{
    public partial class StudentInfo : Window
    {
        GroupEnum groups;
        StudentEnum students;
        Student selectedItem;
        List<StudentInfoPage> myList;
        int selectedIndex;

        public StudentInfo(object _selectedItem, GroupEnum _groups, StudentEnum _students, int _selectedIndex)
        {
            InitializeComponent();
            myList = new List<StudentInfoPage>(); // [0] - first, [1] - last, [2] - next, [3] - prev;
            selectedItem = (Student)_selectedItem;
            groups = _groups;
            students = _students;
            selectedIndex = _selectedIndex;
            Task.Factory.StartNew(() =>
           {
               Application.Current.Dispatcher.BeginInvoke(new Action(() =>
               {
                   //foreach (Student student in students.studentlist)
                   //  myList.Add(new StudentInfoPage(selectedItem, groups, students));

                   myList.Add( new StudentInfoPage(students.studentlist[0], groups, students));
                   myList.Add(new StudentInfoPage(students.studentlist[students.Count() - 1], groups, students));

                   if (selectedIndex != students.studentlist.Count() - 1)
                       myList.Add(new StudentInfoPage(students.studentlist[selectedIndex + 1], groups, students));
                   else
                       myList.Add(new StudentInfoPage(students.studentlist[selectedIndex], groups, students));

                   if(selectedIndex != 0)
                         myList.Add(new StudentInfoPage(students.studentlist[selectedIndex - 1], groups, students));
                   else
                         myList.Add(new StudentInfoPage(students.studentlist[selectedIndex], groups, students));

               }));
           });

            if(students.studentlist.Count == 0)
            {
                Last.IsEnabled = false;
                Next.IsEnabled = false;
                Prev.IsEnabled = false;
                First.IsEnabled = false;
            }
            if (selectedIndex == students.studentlist.Count()-1)
            {
                Last.IsEnabled = false;
                Next.IsEnabled = false;
            }
            if (selectedIndex == 0)
            {
                First.IsEnabled = false;
                Prev.IsEnabled = false;
            }

            newInfo.NavigationService.Navigate(new StudentInfoPage(selectedItem, groups, students));
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex++;
            if (selectedIndex + 1 >= students.studentlist.Count())
            {
                Next.IsEnabled = false;
                Last.IsEnabled = false;
            }
            Prev.IsEnabled = true;
            First.IsEnabled = true;
            newInfo.NavigationService.Navigate(new StudentInfoPage(students.studentlist[selectedIndex], groups, students));
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex--;
            if (selectedIndex - 1 < 0)
            {
                Prev.IsEnabled = false;
                First.IsEnabled = false;
            }
            Next.IsEnabled = true;
            Last.IsEnabled = true;
            newInfo.NavigationService.Navigate(new StudentInfoPage(students.studentlist[selectedIndex], groups, students));
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            Prev.IsEnabled = false;
            First.IsEnabled = false;
            Next.IsEnabled = true;
            Last.IsEnabled = true;
            selectedIndex = 0;
            newInfo.NavigationService.Navigate(new StudentInfoPage(students.studentlist[0], groups, students));
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            Prev.IsEnabled = true;
            First.IsEnabled = true;
            Next.IsEnabled = false;
            Last.IsEnabled = false;
            selectedIndex = students.studentlist.Count() - 1;
            newInfo.NavigationService.Navigate(new StudentInfoPage(students.studentlist[students.studentlist.Count() - 1], groups, students));
        }
    }
}
