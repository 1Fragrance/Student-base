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
    public partial class newSubject : Window
    {
        GroupsWindow prevWindow;
        Group subject;

        public newSubject(GroupsWindow _prevWindow, Group _subject)
        {
            InitializeComponent();

            subject = _subject;
            prevWindow = _prevWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SubjectField.Text.Length > 20 || TeacherField.Text.Length > 20)
            {
                MessageBox.Show("Subject name and Teacher's name must not contain more than 20 letters ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (String.IsNullOrWhiteSpace(TeacherField.Text) || String.IsNullOrWhiteSpace(SubjectField.Text))
            {
                MessageBox.Show("Please, fill all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TeacherField.Clear();
                SubjectField.Clear();
                return;
            }

            Group.struct_teachers res = new Group.struct_teachers
            {
                Name = TeacherField.Text,
                Subject = SubjectField.Text
            };
            subject.teachers.Add(res);

            prevWindow.SubjectGrid.Items.Refresh();
            Close();
        }

        private void SubjectField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) > 0;
        }

        private void TeacherField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) > 0;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
