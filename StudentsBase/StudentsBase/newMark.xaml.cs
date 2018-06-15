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
    
    public partial class newMark : Window
    {
        StudentsWindow prevWindow;
        Student marks;

        public newMark(StudentsWindow _prevWindow, Student _marks)
        {
            InitializeComponent();

            marks = _marks;
            prevWindow = _prevWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SubjectField.Text.Length > 16)
            {
                MessageBox.Show("Subject name must not contain more than 16 letters ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (String.IsNullOrWhiteSpace(MarkField.Text) || String.IsNullOrWhiteSpace(SubjectField.Text))
            {
                MessageBox.Show("Please, fill all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MarkField.Clear();
                SubjectField.Clear();
                return;
            }

            if(Convert.ToInt32(MarkField.Text) > 10 || Convert.ToInt32(MarkField.Text) < 0)
            {
                MessageBox.Show("In the mark field number should be between 0 to 10", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MarkField.Clear();
                return;
            }

            Student.struct_marks res = new Student.struct_marks
            {
                Mark = Convert.ToInt32(MarkField.Text),
                Subject = SubjectField.Text
            };
            marks.marks.Add(res);

            prevWindow.Table.Items.Refresh();
            Close();
        }

        private void SubjectField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) > 0;
        }

        private void MarkField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
