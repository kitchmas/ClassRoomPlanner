using ClassRoomPlanner.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassRoomPlanner.Seating
{
    // Organises the seating plan
    public class SeatPlanner
    {
        private Child currentChild;
        private ObservableCollection<Child> childrenInClass;
        private ObservableCollection<Table<Child>> tablesInClass;
        private List<Child> goodChildrenOnHold { get; set; }
        private List<Child> disruptiveChildrenOnHold { get; set; }
        private const int firstSeatAtTable = 0;
        private const int empty = 0;

        public bool DisruptiveChildrenAreOnHold
        {
            get { return disruptiveChildrenOnHold.Count == 0 ? false : true; }
        }


        private bool endOfClassReached = false;
        private int _childCount;
        public int ChildCount
        {
            get { return _childCount; }
            set
            {
                _childCount = value;
                if (_childCount == childrenInClass.Count)
                    endOfClassReached = true;
            }
        }




        public SeatPlanner(ObservableCollection<Table<Child>> tables, ObservableCollection<Child> children)
        {

            tablesInClass = tables;
            childrenInClass = children;
            goodChildrenOnHold = new List<Child>();
            disruptiveChildrenOnHold = new List<Child>();
            ChildCount = 0;

        }

        // Randomly organise the seating plan 
        public ObservableCollection<Table<Child>> RandomSeating()
        {
            int childCount = 0;
            ClearTables();
            childrenInClass.Shuffle();




            foreach (var table in tablesInClass)
            {

                while (!table.IsTableFull)
                {
                    if (childCount == childrenInClass.Count)
                    {
                        break;
                    }
                    table.AddChild(childrenInClass[childCount]);
                    childCount++;
                }
            }




            return tablesInClass;
        }

        
        //organise the seating plan with the condition that children who disrupt each other can't sit next to each other.
        public ObservableCollection<Table<Child>> DisruptiveCantSitBySeat()
        {

            endOfClassReached = false;
            ChildCount = 0;

            childrenInClass.Shuffle();
            ClearTables();
            SeatChildren();
            FinalPlacement();
            CreateUnplacableCollection();


            return tablesInClass;

        }

        // organise the seating plan with the condition that children who disrupt each other can't sit on the same table
        public ObservableCollection<Table<Child>> DisruptiveCantSitByTable()
        {


            List<Child> disruptiveChildrenList;
            bool tenLoops = false;
            int loopCount = 0;

            do
            {
                ClearTables();
                loopCount++;
                if (loopCount == 10)
                {
                    tenLoops = true;
                }

                childrenInClass.Shuffle();

                var disruptiveChildren = from disruptiveChild in childrenInClass
                                         where disruptiveChild.IsDisruptive
                                         select disruptiveChild;

                disruptiveChildrenList = disruptiveChildren.ToList<Child>();



                foreach (var disruptiveChild in disruptiveChildren)
                {



                    foreach (var table in tablesInClass)
                    {
                        if (table.IsTableFull)
                            break;

                        var disruptiveMatch = from item in disruptiveChild.CantSitWith
                                              join secondItem in table.ChildrenAtTable
                                              on item.Id equals secondItem.Id
                                              select item;

                        if (disruptiveMatch.Any())
                        {
                            continue;
                        }
                        else
                        {
                            table.AddChild(disruptiveChild);
                            disruptiveChildrenList.RemoveAll(c => c.Id == disruptiveChild.Id);
                            break;
                        }


                    }
                }
            } while (disruptiveChildrenList.Any() || tenLoops);

           goodChildrenOnHold= GetGoodChildren();
            PlaceGoodChildren();

            return tablesInClass;
        }

        //Seat the remaining children
        private List<Child> GetGoodChildren()
        {


            var tablesWithSpareSeats = from table in tablesInClass
                                       where !table.IsTableFull
                                       select table;

            var goodChildren = childrenInClass.Where(c => c.IsDisruptive == false).ToList<Child>();
            return goodChildren;
            
            
        }

       
        // clears the tables seat
        private void ClearTables()
        {
            foreach (var table in tablesInClass)
            {
                table.ClearTable();
            }
        }


        //Go through each table and add children one by one, a child will only be placed at table if he will not disrupt the child next to them.
        private void SeatChildren()
        {
            foreach (var table in tablesInClass)
            {
                while (!table.IsTableFull)
                {

                    if (endOfClassReached)
                        break;

                    currentChild = childrenInClass[ChildCount];

                    if (table.isEmpty)
                    {

                        if (!DisruptiveChildrenAreOnHold)
                        {
                            table.ChildrenAtTable.Add(currentChild);
                            ChildCount++;

                            continue;
                        }
                        else
                        {
                            Child firstChildOnHold = disruptiveChildrenOnHold[0];
                            table.AddChild(firstChildOnHold);
                            disruptiveChildrenOnHold.RemoveAt(0);

                            continue;

                        }
                    }


                    if (DisruptiveChildrenAreOnHold)
                        TrySeatDisruptiveChildren(table);
                    if (table.IsTableFull)
                        break;

                    PlaceChild(currentChild, table);




                }
            }


        }

        //Trys to seat all the children on hold
        private void FinalPlacement()
        {

            int loopCount = 0;
       

            while (disruptiveChildrenOnHold.Count != 0)
            {
                loopCount++;
                if (loopCount == 3)
                {
                    break;
                }

                ReSitDisruptiveChildren();

            }
            PlaceGoodChildren();
        }

        //Creates a collection for any children that did not get placed
        private void CreateUnplacableCollection()
        {
            if (disruptiveChildrenOnHold.Count != empty)
            {
                Table<Child> disruptiveTable = new Table<Child>(disruptiveChildrenOnHold.Count, disruptiveChildrenOnHold);
                disruptiveTable.Name = "very disruptive";
            }
        }

        // trys to seat the disruptive children in the class to the current table
        private void TrySeatDisruptiveChildren(Table<Child> table)
        {
            Child childToCheck;
            int seatCount = table.SeatsOccupiedCount;
            for (int dc = disruptiveChildrenOnHold.Count - 1; dc >= 0; dc--)
            {
                try
                {

                    childToCheck = table.ChildrenAtTable.Last();
                    Child disruptiveChild = disruptiveChildrenOnHold[dc];

                    if ((childToCheck.IsDisruptive && childToCheck.CantSitWith.Any(c => c.Id == disruptiveChild.Id))
                        || disruptiveChild.CantSitWith.Any(c => c.Id == childToCheck.Id))
                    {
                        continue;
                    }
                    else
                    {
                        table.AddChild(disruptiveChild);
                        seatCount++;
                        disruptiveChildrenOnHold.RemoveAt(dc);
                        if (seatCount == table.NumberOfChairs)
                        {
                            break;


                        }

                    }
                }
                catch (System.ArgumentOutOfRangeException e) { Debug.WriteLine(e.ToString() + "154 "); }
                catch (System.ArgumentNullException e) { Debug.WriteLine(e.ToString() + "154 "); }
                catch (System.ArgumentException e) { Debug.WriteLine(e.ToString() + "154 "); }



            }


        }

        //Places the good children in the class
        private void PlaceGoodChildren()
        {
            var tablesWithSpareSeats = from table in tablesInClass
                                       where !table.IsTableFull
                                       select table;

            foreach (var table in tablesWithSpareSeats)
            {

                while (!table.IsTableFull)
                {
                    if (goodChildrenOnHold.Count != empty)
                    {
                        Child goodChild = goodChildrenOnHold.Last();
                        table.AddChild(goodChild);
                        goodChildrenOnHold.RemoveAll(c => c.Id == goodChild.Id);
                    }
                    else { break; }
                }

            }

        }

        //Check if there any disruptive children left and try to replace them
        private void ReSitDisruptiveChildren()
        {

            for (int dc = disruptiveChildrenOnHold.Count - 1; dc >= 0; dc--)
            {

                Child disruptiveChild = disruptiveChildrenOnHold[dc];

                foreach (var table in tablesInClass)
                {
                    if (table.IsTableFull)
                    {
                        if (TrySwapSeats(disruptiveChild, table))
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (TrySeatAtTableWithSpareSeats(disruptiveChild, table))
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }
    
        // checks if a disruptive child can sit at a table with spare seats and sits them
        private bool TrySeatAtTableWithSpareSeats(Child disruptiveChild, Table<Child> table)
        {
          
                if (table.isEmpty)
                {
                    table.ChildrenAtTable.Add(disruptiveChild);
                    disruptiveChildrenOnHold.RemoveAll(c => c.Id == disruptiveChild.Id);
                return true;
               
                }
                else if (table.ChildrenAtTable.Count != table.NumberOfChairs)
                {
                    Child childToCheck = table.ChildrenAtTable.Last();
                    if (!childToCheck.IsDisruptive || !childToCheck.CantSitWith.Any(c => c.Id == disruptiveChild.Id))
                    {
                        table.ChildrenAtTable.Add(disruptiveChild);
                    disruptiveChildrenOnHold.RemoveAll(c => c.Id == disruptiveChild.Id);

                    return true;
                    }
                }

            return false;
            }

        // checks if a disruptive be replaced with a  good child at a full table and swaps them
        private bool TrySwapSeats(Child disruptiveChild, Table<Child> table)
        {
            int lastSeatAtTable = table.NumberOfChairs - 1;
            for (int seatCount = 0; seatCount < table.NumberOfChairs; seatCount++)
            {

                Child childToCheck = table.ChildrenAtTable[seatCount];

                if (seatCount == firstSeatAtTable)
                {
                    Child childSatAfterChildToCheck = table.ChildrenAtTable[seatCount + 1];

                    if (!childToCheck.IsDisruptive &&
                        (!childSatAfterChildToCheck.IsDisruptive || !childSatAfterChildToCheck.CantSitWith.Any(c => c.Id == disruptiveChild.Id))
                        && !disruptiveChild.CantSitWith.Any(c => c.Id == childSatAfterChildToCheck.Id))
                    {
                        goodChildrenOnHold.Add(childToCheck);
                        table.ChildrenAtTable[seatCount] = disruptiveChild;
                        disruptiveChildrenOnHold.RemoveAll(c => c.Id == disruptiveChild.Id);

                        return true;

                    }


                }
                // check to see if it's the last seat in the table
                else if (seatCount == lastSeatAtTable)
                {
                    Child childSatBeforeChildToCheck = table.ChildrenAtTable[seatCount - 1];

                    if (!childToCheck.IsDisruptive &&
                        (!childSatBeforeChildToCheck.IsDisruptive || !childSatBeforeChildToCheck.CantSitWith.Any(c => c.Id == disruptiveChild.Id))
                        && !disruptiveChild.CantSitWith.Any(c => c.Id == childSatBeforeChildToCheck.Id))
                    {
                        goodChildrenOnHold.Add(childToCheck);
                        table.ChildrenAtTable[seatCount] = disruptiveChild;
                        disruptiveChildrenOnHold.RemoveAll(c => c.Id == disruptiveChild.Id);

                        return true;
                    }
                }
                // otherwise it must be a seat inbetween two other seats
                else
                {
                    Child childSatBeforeChildToCheck = table.ChildrenAtTable[seatCount - 1];
                    Child childSatAfterChildToCheck = table.ChildrenAtTable[seatCount + 1];
                    if (!childToCheck.IsDisruptive && (!childSatBeforeChildToCheck.IsDisruptive ||
                        !childSatBeforeChildToCheck.CantSitWith.Any(c => c.Id == disruptiveChild.Id))
                        && (!childSatAfterChildToCheck.IsDisruptive ||
                        !childSatAfterChildToCheck.CantSitWith.Any(c => c.Id == disruptiveChild.Id))
                        && !disruptiveChild.CantSitWith.Any(c => c.Id == childSatBeforeChildToCheck.Id)
                        && !disruptiveChild.CantSitWith.Any(c => c.Id == childSatAfterChildToCheck.Id))
                    {
                        goodChildrenOnHold.Add(childToCheck);
                        table.ChildrenAtTable[seatCount] = disruptiveChild;
                        disruptiveChildrenOnHold.RemoveAll(c => c.Id == disruptiveChild.Id);

                        return true;
                    }
                }
            }
            return false;
        }
    
        //places  a child  at a table
        private void PlaceChild(Child child, Table<Child> table)
        {
            Child childToCheck = table.ChildrenAtTable.Last();


            if (child.IsDisruptive)
            {

                if ((childToCheck.IsDisruptive && childToCheck.CantSitWith.Any(c => c.Id == child.Id)) || child.CantSitWith.Any(c => c.Id == child.Id))
                {
                    disruptiveChildrenOnHold.Add(child);



                }
                else
                {
                    table.AddChild(child);


                }

            }
            else if (childToCheck.IsDisruptive && childToCheck.CantSitWith.Any(c => c.Id == child.Id))
            {

                goodChildrenOnHold.Add(child);


            }
            else
            {
                table.AddChild(child);

            }
            ChildCount++;
        }
















    }
}















