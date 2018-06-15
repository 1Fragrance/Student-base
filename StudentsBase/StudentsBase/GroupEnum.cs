using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsBase
{
    public class GroupEnum : IEnumerable<Group>
    {

        public List<Group> groupList;

        public GroupEnum()
        {
            groupList = new List<Group>();
        }

        public IEnumerator<Group> GetEnumerator()
        {
            return new GroupListEnum(groupList);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class GroupListEnum : IEnumerator<Group>
        {
            List<Group> group_list;

            private Group current_element;
            private int current_index = -1;

            public GroupListEnum(List<Group> group_list)
            {
                this.group_list = group_list;
            }

            public void Dispose() { }

            public void Reset()
            {
                current_index = -1;
            }

            public bool MoveNext()
            {
                current_index++;
                if (current_index >= group_list.Count)
                {
                    current_element = null;
                    return false;
                }
                else
                {
                    current_element = group_list[current_index];
                    return true;
                }
            }

            public Group Current => current_element;
            object IEnumerator.Current => Current;
        }
    }
}
