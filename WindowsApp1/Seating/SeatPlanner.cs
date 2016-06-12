using ClassRoomPlanner.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassRoomPlanner.Seating
{
    // Organises the seating plan
    public class SeatPlanner
    {
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
        public ObservableCollection<Table<Child>> GenerateCantSitWithSeating(ObservableCollection<Table<Child>> tables, ObservableCollection<Child> children)
        {
            int currentChildCount = 0;

            children.Shuffle();

            var childrenOnHold = new List<Child>();
            int newTable = 0;
            int empty = 0;
            foreach (var table in tables)
            {
                table.ChildrenAtTable.Clear();
                for (int i = 0; i < table.NumberOfChairs; i++)
                {
                    if (currentChildCount >= children.Count)
                    {
                        break;
                    }
                    else
                    {

                        Child currentChild = children[currentChildCount];
                        currentChildCount++;
                        if (i == newTable)
                        {
                            if (childrenOnHold.Count == empty)
                            {
                                table.ChildrenAtTable.Add(currentChild);
                                continue;

                            }
                            else
                            {
                                var firstChildOnHold = childrenOnHold[0];
                                table.ChildrenAtTable.Add(firstChildOnHold);
                                childrenOnHold.RemoveAll(c => c.Id == firstChildOnHold.Id);
                            } // add check to see if  current child is disruptive 
                        }
                        else if (childrenOnHold.Count != empty)
                        {


                            for (int j = childrenOnHold.Count - 1; j >= 0; j--)
                            {
                                var childToCheck = table.ChildrenAtTable.Last();
                                var naughtyChild = childrenOnHold[j];

                                if ((childToCheck.IsDisruptive && childToCheck.cantSitWith.Any(c => c.Id == naughtyChild.Id)) || naughtyChild.cantSitWith.Any(c => c.Id == childToCheck.Id))
                                {
                                    continue;
                                }
                                else
                                {
                                    table.ChildrenAtTable.Add(naughtyChild);
                                    childrenOnHold.RemoveAt(j);
                                }

                            }
                        }


                        if (currentChild.IsDisruptive)
                        {
                            var childToCheck = table.ChildrenAtTable.Last();
                            if ((childToCheck.IsDisruptive && childToCheck.cantSitWith.Any(c => c.Id == currentChild.Id)) || currentChild.cantSitWith.Any(c => c.Id == childToCheck.Id))
                            {
                                childrenOnHold.Add(currentChild);
                                continue;
                            }
                            else
                            {
                                table.ChildrenAtTable.Add(currentChild);
                                continue;
                            }
                        }
                        table.ChildrenAtTable.Add(currentChild);
                    }
                }



            }
            if (childrenOnHold.Count != 0)
            {

                //    //Loop through the class again up to 3 times maximum(to stop and infinite loop) and see if you can swap a good child out

                //    //check current child is not disruptive and then check to see if the child either side can sit with the child
                //    // if so take the good child out insert the naughty child and at the end place the remainding good children at the back
                //    //of the class.
                bool stopLooping = false;
                int loopCount = 0;

                //Add check somewhere to see if childrenOnHold is empty when empty add all good children to end of the table collection 
                //maybe check if the the last seat is empty?????????????????????????????????????????????????

                List<Child> goodChildrenHoldList = new List<Child>();
                while (childrenOnHold.Count != 0 || stopLooping)
                {
                    loopCount++;
                    if (loopCount == 3)
                    {
                        stopLooping = true;
                    }



                    for (int k = childrenOnHold.Count - 1; k >= 0; k--)
                    {
                        if (childrenOnHold.Count == 0)
                            break;
                        foreach (var ttable in tables)
                        {
                            if (childrenOnHold.Count == 0)
                                break;
                            for (int a = 0; a < ttable.ChildrenAtTable.Count; a++)
                            {
                                if (childrenOnHold.Count == 0)
                                    break;
                                Child naughtyChild = childrenOnHold[k];
                                Child childToCheck = ttable.ChildrenAtTable[a];

                                //check to see if it's the first seat in the tabe
                                if (a == 0)
                                {
                                    Child childSatAfterChildToCheck = ttable.ChildrenAtTable[a + 1];
                                    // null exception because it's checking if the an empty list contain data dont check if they are both disruptive!!!!
                                    if (!childToCheck.IsDisruptive && (!childSatAfterChildToCheck.IsDisruptive || !childSatAfterChildToCheck.cantSitWith.Any(c => c.Id == naughtyChild.Id)) && !naughtyChild.cantSitWith.Any(c => c.Id == childSatAfterChildToCheck.Id))
                                    {
                                        goodChildrenHoldList.Add(childToCheck);
                                        ttable.ChildrenAtTable[a] = naughtyChild;
                                        childrenOnHold.RemoveAt(k);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                // check to see if it's the last seat in the table
                                else if (a == childrenOnHold.Count - 1)
                                {
                                    Child childSatBeforeChildToCheck = ttable.ChildrenAtTable[--a];

                                    if ((!childToCheck.IsDisruptive && (!childSatBeforeChildToCheck.IsDisruptive) || !childSatBeforeChildToCheck.cantSitWith.Any(c => c.Id == naughtyChild.Id)) && !naughtyChild.cantSitWith.Any(c => c.Id == childSatBeforeChildToCheck.Id))
                                    {
                                        goodChildrenHoldList.Add(childToCheck);
                                        ttable.ChildrenAtTable[a] = naughtyChild;
                                        childrenOnHold.RemoveAt(k);
                                    }
                                }
                                // otherwise it must be a seat inbetween to other seats
                                else
                                {
                                    Child childSatBeforeChildToCheck = ttable.ChildrenAtTable[--a];
                                    Child childSatAfterChildToCheck = ttable.ChildrenAtTable[++a];
                                    if (!childToCheck.IsDisruptive && (!childSatBeforeChildToCheck.IsDisruptive ||
                                        !childSatBeforeChildToCheck.cantSitWith.Any(c => c.Id == naughtyChild.Id))
                                        && (!childSatAfterChildToCheck.IsDisruptive ||
                                        !childSatAfterChildToCheck.cantSitWith.Any(c => c.Id == naughtyChild.Id))
                                        && !naughtyChild.cantSitWith.Any(c => c.Id == childSatBeforeChildToCheck.Id)
                                        && !naughtyChild.cantSitWith.Any(c => c.Id == childSatAfterChildToCheck.Id))
                                    {
                                        goodChildrenHoldList.Add(childToCheck);
                                        ttable.ChildrenAtTable[a] = naughtyChild;
                                        childrenOnHold.RemoveAt(k);
                                    }
                                    else
                                    {
                                        //should be okay to just continue
                                        continue;

                                    }

                                }


                            }



                        }
                    }

                }
                //Now that the while loop has finished we need to add all the good chidren on hold back to the end of the class

                //Think this logic is sorted check tommorow!
                foreach (var ttable in tables)
                {
                    if (ttable.ChildrenAtTable.Count == ttable.NumberOfChairs)
                    {
                        continue;
                    }
                    else
                    {

                        Child ChildGoingToNextTable = null;

                        foreach (var child in goodChildrenHoldList)
                        {
                            if (ChildGoingToNextTable != null)
                            {
                                ttable.ChildrenAtTable.Add(ChildGoingToNextTable);
                                ChildGoingToNextTable = null;
                            }
                            if (ttable.ChildrenAtTable.Count == ttable.NumberOfChairs)
                            {
                                ChildGoingToNextTable = child;
                                continue;
                            }
                            else
                            {
                                ttable.ChildrenAtTable.Add(child);
                                continue;
                            }
                        }
                    }
                }


            }
            return tables;
        }


    }
}















