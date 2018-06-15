using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;


namespace StudentsBase
{
    public class Student
    {
        public struct struct_marks
        {
            public string Subject { get; set; }
            public int Mark { get; set; }
        }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Middlename { get; set; }
        public int Year { get; set; }

        public string isHead { get; set; }
        public string groupHeading { get; set; }

        public Group group;
        public string group_number { get; set; }

        public Group oldGroup;
        public string old_group_number { get; set; }

        public BitmapImage studentPhoto { get;set; }
        public string photoPath { get; set; }

        public List<struct_marks> marks = new List<struct_marks>();
    }
}
