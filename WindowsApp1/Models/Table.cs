using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassRoomPlanner.Model
{
   public class Table<T>
    {
        private static int idCount = 0;
        
        public int NumberOfChairs { get; set; }
        private int id;
        public int Id
        {
            get { return id; }
        }
        public string Name { get; }
        public List<T> ChildrenAtTable { get; set; }

        public Table(int numberOfChairs, List<T> childrenAtTable)
        {
            idCount++;
            NumberOfChairs = numberOfChairs;
            id = idCount;
            Name = string.Format("Table {0}", id);
            
            ChildrenAtTable = childrenAtTable;
        }

        public Table(int numberOfChairs)
        {
            idCount++;
            NumberOfChairs = numberOfChairs;
            id = idCount;
            Name = string.Format("Table {0}", id);
            ChildrenAtTable = new List<T>();
        }
    }
}
