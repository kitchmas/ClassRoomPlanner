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
        public int ChildCount {
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
        public ObservableCollection<Table<Child>> GenerateRandomSeating(ObservableCollection<Table<Child>> tables, ObservableCollection<Child> children)
        {
            int totalSeats = 0;
            foreach (var table in tables)
            {
                table.ChildrenAtTable.Clear();
                totalSeats += table.NumberOfChairs;
            }
            if (totalSeats < children.Count)
                throw new ArgumentOutOfRangeException("tables", tables, "Not enough seats");

            children.Shuffle();
            int nextChild = 0;

            foreach (var Table in tables)
            {
                for (int i = 0; i < Table.NumberOfChairs; i++)
                {
                    Table.ChildrenAtTable.Add(children[nextChild]);
                    nextChild++;
                    if (nextChild >= children.Count)
                        return tables;
                }
            }

            return tables;
        }

        //Still have an issue where sometimes the number of seats at each table changes. Each table should only hold as many seats as it has .
        // Arranges the seating plan with the condition that children who disrupt each other can't sit next to each other.
        public ObservableCollection<Table<Child>> GenerateCantSitWithSeating()
        {

            childrenInClass.Shuffle();
            ClearTables();
            SeatChildren();
            FinalPlacement();
            CreateUnplacableCollection();


            return tablesInClass;

        }


        private void ClearTables()
        {
            foreach (var table in tablesInClass)
            {
                table.ChildrenAtTable.Clear();
            }
        }

        
      

        //Go through each table and add children one by one, a child will only be placed at table if he will not disrupt the child next to them.
        private void SeatChildren()
        {
            foreach (var table in tablesInClass)
            {
               while(table.ChildrenAtTable.Count != table.NumberOfChairs )
                {

                    if (endOfClassReached)
                        break;

                    currentChild = childrenInClass[ChildCount];

                    if (table.ChildrenAtTable.Count == firstSeatAtTable)
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
                            table.ChildrenAtTable.Add(firstChildOnHold);
                            disruptiveChildrenOnHold.RemoveAt(0);

                            continue;

                        }
                    }


                    if (DisruptiveChildrenAreOnHold)
                        TrySeatDisruptiveChildren(table);
                    if (table.NumberOfChairs == table.ChildrenAtTable.Count)
                        break;

                    PlaceChild(currentChild,table);
                    
                  


                }
            }

            
        }

        private void FinalPlacement()
        {

            int loopCount = 0;
            //    //Loop through the class again up to 3 times maximum(to stop and infinite loop) and see if you can swap a good child out

            //    //check current child is not disruptive and then check to see if the child either side can sit with the child
            //    // if so take the good child out insert the naughty child and at the end place the remainding good children at the back
            //    //of the class.
           


                //Add check somewhere to see if childrenOnHold is empty when empty add all good children to end of the table collection 
                //maybe check if the the last seat is empty?????????????????????????????????????????????????


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

        private void CreateUnplacableCollection()
        {
            if (disruptiveChildrenOnHold.Count != empty)
            {
                Table<Child> disruptiveTable = new Table<Child>(disruptiveChildrenOnHold.Count, disruptiveChildrenOnHold);
                disruptiveTable.Name = "very disruptive";
            }
        }


        //Check the hold collections and try and place the remaining children
        // trys to seat the disruptive children in the class to the current table.
        private void TrySeatDisruptiveChildren(Table<Child> table)
        {
            Child childToCheck;
            int seatCount = table.ChildrenAtTable.Count;
            for (int dc = disruptiveChildrenOnHold.Count - 1; dc >= 0; dc--)
            {
                try
                {

                    childToCheck = table.ChildrenAtTable.Last();
                    Child disruptiveChild = disruptiveChildrenOnHold[dc];

                    if ((childToCheck.IsDisruptive && childToCheck.cantSitWith.Any(c => c.Id == disruptiveChild.Id))
                        || disruptiveChild.cantSitWith.Any(c => c.Id == childToCheck.Id))
                    {
                        continue;
                    }
                    else
                    {
                        table.ChildrenAtTable.Add(disruptiveChild);
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
       
        private void PlaceGoodChildren()
        {
            var tablesWithSpareSeats = from table in tablesInClass
                                       where table.ChildrenAtTable.Count != table.NumberOfChairs
                                       select table;

            foreach (var table in tablesWithSpareSeats)
            {
                
                while(table.ChildrenAtTable.Count != table.NumberOfChairs)
                {
                    if (goodChildrenOnHold.Count != empty)
                    {
                        Child goodChild = goodChildrenOnHold.Last();
                        table.ChildrenAtTable.Add(goodChild);
                        goodChildrenOnHold.RemoveAll(c => c.Id == goodChild.Id);
                    }else { break; }
                }
              
            }
            
        }

        

        private void ReSitDisruptiveChildren()
        {
           
                
            

            for (int dc = disruptiveChildrenOnHold.Count - 1; dc >= 0; dc--)
            {
                if (disruptiveChildrenOnHold.Count == empty)
                    break;

                Child disruptiveChild = disruptiveChildrenOnHold[dc];
                foreach (var table in tablesInClass)
                {
                    if (disruptiveChildrenOnHold.Count == empty)
                        break;
                    if (table.ChildrenAtTable.Count == empty)
                    {
                        table.ChildrenAtTable.Add(disruptiveChild);
                        disruptiveChildrenOnHold.RemoveAt(dc);
                        continue;
                    }
                    else if (table.ChildrenAtTable.Count != table.NumberOfChairs)
                    {
                        Child childToCheck = table.ChildrenAtTable.Last();
                        if (!childToCheck.IsDisruptive || !childToCheck.cantSitWith.Any(c => c.Id == disruptiveChild.Id))
                        {
                            table.ChildrenAtTable.Add(disruptiveChild);
                            disruptiveChildrenOnHold.RemoveAt(dc);
                            continue;
                        }
                    }
                    else 
                    {
                        int lastSeatAtTable = table.NumberOfChairs - 1;

                        for (int seatCount = 0; seatCount < table.NumberOfChairs;)
                        {
                            if (disruptiveChildrenOnHold.Count == 0)
                                break;

                            // so here we want to go through each seat and try and swap out the good kids with naughty kids where and when
                            // i guess if table.count = 0 we wan't to break the  loop ? or add 1 dc then cut the loop? shit me!


                            //seat is causing erros it's not a thing

                            Child childToCheck = table.ChildrenAtTable[seatCount];


                            if (seatCount == firstSeatAtTable)
                            {
                                Child childSatAfterChildToCheck = table.ChildrenAtTable[seatCount + 1];

                                if (!childToCheck.IsDisruptive &&
                                    (!childSatAfterChildToCheck.IsDisruptive || !childSatAfterChildToCheck.cantSitWith.Any(c => c.Id == disruptiveChild.Id))
                                    && !disruptiveChild.cantSitWith.Any(c => c.Id == childSatAfterChildToCheck.Id))
                                {
                                    goodChildrenOnHold.Add(childToCheck);
                                    table.ChildrenAtTable[seatCount] = disruptiveChild;
                                    disruptiveChildrenOnHold.RemoveAt(dc);
                                    continue;
                                }// but what happens if I can't sit the child? Next check????


                            }
                            // check to see if it's the last seat in the table
                            else if (seatCount == lastSeatAtTable)
                            {
                                Child childSatBeforeChildToCheck = table.ChildrenAtTable[seatCount - 1];

                                if ((!childToCheck.IsDisruptive &&
                                    (!childSatBeforeChildToCheck.IsDisruptive) || !childSatBeforeChildToCheck.cantSitWith.Any(c => c.Id == disruptiveChild.Id))
                                    && !disruptiveChild.cantSitWith.Any(c => c.Id == childSatBeforeChildToCheck.Id))
                                {
                                    goodChildrenOnHold.Add(childToCheck);
                                    table.ChildrenAtTable[seatCount] = disruptiveChild;
                                    disruptiveChildrenOnHold.RemoveAt(dc);
                                    continue;
                                }
                            }
                            // otherwise it must be a seat inbetween two other seats
                            else
                            {
                                Child childSatBeforeChildToCheck = table.ChildrenAtTable[seatCount - 1];
                                Child childSatAfterChildToCheck = table.ChildrenAtTable[seatCount + 1];
                                if (!childToCheck.IsDisruptive && (!childSatBeforeChildToCheck.IsDisruptive ||
                                    !childSatBeforeChildToCheck.cantSitWith.Any(c => c.Id == disruptiveChild.Id))
                                    && (!childSatAfterChildToCheck.IsDisruptive ||
                                    !childSatAfterChildToCheck.cantSitWith.Any(c => c.Id == disruptiveChild.Id))
                                    && !disruptiveChild.cantSitWith.Any(c => c.Id == childSatBeforeChildToCheck.Id)
                                    && !disruptiveChild.cantSitWith.Any(c => c.Id == childSatAfterChildToCheck.Id))
                                {
                                    goodChildrenOnHold.Add(childToCheck);
                                    table.ChildrenAtTable[seatCount] = disruptiveChild;
                                    disruptiveChildrenOnHold.RemoveAt(dc);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PlaceChild(Child child, Table<Child> table)
        {
            Child childToCheck = table.ChildrenAtTable.Last();


            if (child.IsDisruptive)
            {

                if ((childToCheck.IsDisruptive && childToCheck.cantSitWith.Any(c => c.Id == child.Id)) || child.cantSitWith.Any(c => c.Id == child.Id))
                {
                    disruptiveChildrenOnHold.Add(child);
     
                 
                    
                }
                else
                {
                    table.ChildrenAtTable.Add(child);
                
                
                }

            }
            else if (childToCheck.IsDisruptive && childToCheck.cantSitWith.Any(c => c.Id == child.Id))
            {

                goodChildrenOnHold.Add(child);
               
                
            }
            else
            {
                table.ChildrenAtTable.Add(child);
               
            }
            ChildCount++;
        }
    
        
    
         


        


        



        


    }
}















