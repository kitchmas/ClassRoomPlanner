using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassRoomPlanner.Model
{
   public class Table<T>
    {
        //fix bug where id count starts again on serilization
        private static int idCount = 0;
        
        public int NumberOfChairs { get; set; }
        public int Id { get; set; }
    
        public string Name { get; set; }
        public List<T> ChildrenAtTable { get; set; }
        public Table() { }

        public Table(int numberOfChairs, List<T> childrenAtTable)
        {
            idCount++;
            NumberOfChairs = numberOfChairs;
            Id = idCount;
            Name = string.Format("Table {0}", Id);
            
            ChildrenAtTable = childrenAtTable;
        }

        public Table(int numberOfChairs)
        {
            idCount++;
            NumberOfChairs = numberOfChairs;
            Id = idCount;
            Name = string.Format("Table {0}", Id);
            ChildrenAtTable = new List<T>();
        }
    }
}
