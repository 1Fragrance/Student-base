using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsBase 
{
    public class StudentEnum : IEnumerable<Student>
    {
        public List<Student> studentlist;

        public StudentEnum()
        {
            studentlist = new List<Student>();
        }

        public IEnumerator<Student> GetEnumerator()
        {
            return new StudentListEnumerator(studentlist);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class StudentListEnumerator : IEnumerator<Student>
        {
            List<Student> student_list;

            private Student current_element;
            private int current_index = -1;

            public StudentListEnumerator(List<Student> student_list)
            {
                this.student_list = student_list;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                current_index++;
                if (current_index >= student_list.Count)
                {
                    current_element = null;
                    return false;
                }

                current_element = student_list[current_index];
                return true;
            }

            public void Reset()
            {
                current_index = -1;
            }

            public Student Current => current_element;
            object IEnumerator.Current => Current;
        }
    }
}
