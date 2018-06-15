using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class GroupInfo : Window
    {
        GroupEnum groups;
        StudentEnum students;
        Group selectedItem;
        List<GroupInfoPage> myList;
        int selectedIndex;

        public GroupInfo(object _selectedItem, GroupEnum _groups, StudentEnum _students, int _selectedIndex)
        {
            InitializeComponent();
            myList = new List<GroupInfoPage>();

            selectedIndex = _selectedIndex;
            selectedItem = (Group)_selectedItem;
            groups = _groups;
            students = _students;

            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {                 
                    myList.Add(new GroupInfoPage(groups.groupList[0], groups, students));
                    myList.Add(new GroupInfoPage(groups.groupList[groups.Count() - 1], groups, students));

                    if (selectedIndex != groups.groupList.Count() - 1)
                        myList.Add(new GroupInfoPage(groups.groupList[selectedIndex + 1], groups, students));
                    else
                        myList.Add(new GroupInfoPage(groups.groupList[selectedIndex], groups, students));

                    if (selectedIndex != 0)
                        myList.Add(new GroupInfoPage(groups.groupList[selectedIndex - 1], groups, students));
                    else
                        myList.Add(new GroupInfoPage(groups.groupList[selectedIndex], groups, students));
                }));
            });

            if (groups.groupList.Count == 0)
            {
                Last.IsEnabled = false;
                Next.IsEnabled = false;
                Prev.IsEnabled = false;
                First.IsEnabled = false;
            }
            if (selectedIndex == groups.groupList.Count() - 1)
            {
                Last.IsEnabled = false;
                Next.IsEnabled = false;
            }
            if (selectedIndex == 0)
            {
                First.IsEnabled = false;
                Prev.IsEnabled = false;
            }

            newInfo.NavigationService.Navigate(new GroupInfoPage(selectedItem, groups, students));
        }

        private bool filter(object item)
        {
            Student mystudent = item as Student;
            return (mystudent.group_number == Convert.ToString(selectedItem.Number));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
           ICollectionView view = CollectionViewSource.GetDefaultView(students.studentlist);
           view.Filter = null;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex++;
            if (selectedIndex + 1 >= groups.groupList.Count())
            {
                Next.IsEnabled = false;
                Last.IsEnabled = false;
            }
            Prev.IsEnabled = true;
            First.IsEnabled = true;
            newInfo.NavigationService.Navigate(new GroupInfoPage(groups.groupList[selectedIndex], groups, students));
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
            newInfo.NavigationService.Navigate(new GroupInfoPage(groups.groupList[selectedIndex], groups, students));
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            Prev.IsEnabled = true;
            First.IsEnabled = true;
            Next.IsEnabled = false;
            Last.IsEnabled = false;
            selectedIndex = groups.groupList.Count() - 1;
            newInfo.NavigationService.Navigate(new GroupInfoPage(groups.groupList[groups.groupList.Count() - 1], groups, students));
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            Prev.IsEnabled = false;
            First.IsEnabled = false;
            Next.IsEnabled = true;
            Last.IsEnabled = true;
            selectedIndex = 0;
            newInfo.NavigationService.Navigate(new GroupInfoPage(groups.groupList[0], groups, students));
        }
    }
}

