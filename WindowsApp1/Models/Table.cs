using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassRoomPlanner.Model
{
   public class Table<T>
    {
        //fix bug where id count starts again on serilization
        

        private static int m_Counter = 0;
        public int NumberOfChairs { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isEmpty { get { return _childrenAtTable.Count == 0 ? true : false; } }
        public int SeatsOccupiedCount { get { return _childrenAtTable.Count; } }
        public int SeatsNotOccupiedCount { get { return NumberOfChairs - _childrenAtTable.Count; } }
        public bool IsTableFull { get { return _childrenAtTable.Count == NumberOfChairs ? true : false; } }
        private ObservableCollection<Child> _childrenAtTable;
        public ObservableCollection<Child> ChildrenAtTable
        {
            get { return _childrenAtTable; }
            set { _childrenAtTable = value; }
        }

        public void ClearTable()
        {
            ChildrenAtTable.Clear();
        }


        public Table() {
           
        }

        public Table(int numberOfChairs)
        {
            this.Id = System.Threading.Interlocked.Increment(ref m_Counter);
            NumberOfChairs = numberOfChairs;

            Name = string.Format("Table {0}", Id);
            ChildrenAtTable = new ObservableCollection<Child>();
        }

        public Table(string name, int numberOfChairs)
        {
            this.Id = System.Threading.Interlocked.Increment(ref m_Counter);
            NumberOfChairs = numberOfChairs;

            Name = name;
            ChildrenAtTable = new ObservableCollection<Child>();
        }

        public Table(int numberOfChairs, List<Child> childrenAtTable)
        {
            this.Id = System.Threading.Interlocked.Increment(ref m_Counter);
            NumberOfChairs = numberOfChairs;
       
            Name = string.Format("Table {0}", Id);
            
            ChildrenAtTable = new ObservableCollection<Child>(childrenAtTable);
        }



        public ObservableCollection<Child> GetChildren()
        {
            return ChildrenAtTable;
        }

        public bool AddChild(Child child)
        {
            if (!this.IsTableFull)
            {
                this.ChildrenAtTable.Add(child);
                return true;
            }
            else
            {
                return false;
            }
        }

   
    }
}
