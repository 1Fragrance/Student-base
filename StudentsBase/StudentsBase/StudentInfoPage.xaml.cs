using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudentsBase
{
    public partial class StudentInfoPage : Page
    {
        GroupEnum groups;
        StudentEnum students;
        Student selectedItem;

        public StudentInfoPage(object _selectedItem, GroupEnum _groups, StudentEnum _students)
        {
            InitializeComponent();

            selectedItem = (Student)_selectedItem;
            groups = _groups;
            students = _students;

            StMarks.ItemsSource = selectedItem.marks;

            StudentName.Text = "Name: " + Convert.ToString(selectedItem.Name);
            StudentSurname.Text = "Surname: " + Convert.ToString(selectedItem.Surname);
            StudentMiddlename.Text = "Middlename: " + Convert.ToString(selectedItem.Middlename);
            StudentYear.Text = "Year of recept: " + Convert.ToString(selectedItem.Year);

            if (selectedItem.group_number == "-")
                StudentGroup.Text = "Group : Not Selected";
            else
                StudentGroup.Text = "Group : " + selectedItem.group_number;

            if (selectedItem.old_group_number == "-")
                StudentOldGroup.Text = "Old Group : Not Selected";
            else
                StudentOldGroup.Text = "Old Group: " + selectedItem.old_group_number;

            if (selectedItem.isHead == "+")
                StudentisHead.Text = "Head: Yes";
            else
                StudentisHead.Text = "Head: No";

            if (selectedItem.studentPhoto != null)
                StudentImage.Source = selectedItem.studentPhoto;
            else
            {
                var bitmap = StudentsBaseResources.noPhoto; 
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    StudentImage.Source = bitmapimage;
                }
                //StudentImage.Source = StudentsBaseResources.noPhoto;
            }
        }
    }
}
