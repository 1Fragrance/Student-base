using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
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
    public partial class GroupInfoPage : Page
    {
        GroupEnum groups;
        StudentEnum students;
        Group selectedItem;
        public GroupInfoPage(object _selectedItem, GroupEnum _groups, StudentEnum _students)
        {
            InitializeComponent();
            selectedItem = (Group)_selectedItem;
            groups = _groups;
            students = _students;
            StudentsList.ItemsSource = students.studentlist;
            TeachersList.ItemsSource = selectedItem.teachers;
            ICollectionView view = CollectionViewSource.GetDefaultView(StudentsList.ItemsSource);
            view.Filter = null;
            view.Filter = filter;

            NumberGr.Text = "Students of the group: " + selectedItem.Number;
        }

        private bool filter(object item)
        {
            Student mystudent = item as Student;
            return (mystudent.group_number == Convert.ToString(selectedItem.Number));
        }
    }
}
