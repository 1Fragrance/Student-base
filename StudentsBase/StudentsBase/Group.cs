using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsBase
{
    public class Group
    {
        public struct struct_teachers
        {
            public string Name { get; set; }
            public string Subject { get; set; }
        }

        public int Number { get; set; }
        public Student head;
        public string headName { get; set; }        

        public string Actual { get; set; }
        public List<struct_teachers> teachers = new List<struct_teachers>();
    }
}
