using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassRoomPlanner.Model
{
    // Organises the seating plan
   public static class SeatPlanner
    {
        // Randomly organise the seating plan 
        public static List<Table<Child>> GenerateRandomSeating(List<Table<Child>> tables, List<Child> children)
        {
            int totalSeats =0;
            foreach(var table in tables)
            {
                totalSeats += table.NumberOfChairs;
            }
            if (totalSeats != children.Count-1)
                throw new ArgumentOutOfRangeException("tables", tables, "Not enough seats");

            children.Shuffle(); 
            int nextChild = 0;

           foreach(Table<Child> Table in tables)
            {
                for (int i = 0; i <Table.NumberOfChairs; i++)
                {
                    Table.ChildrenAtTable.Add(children[nextChild]);
                    nextChild++;
                    if (nextChild == children.Count-1)
                        return tables;
                }
            }

            return tables;  
        }

 
        
    }
}
