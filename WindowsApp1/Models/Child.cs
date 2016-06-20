using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Runtime.Serialization;

namespace ClassRoomPlanner.Model
{
    
    public class Child 
    {
        //fix bug where id count starts again on serilization

        public static int m_Counter;

        public string Name { get; set; }
       
        public int Id { get; set; }
       
        
        
        public Boolean IsDisruptive { get; set; }
        public int _naughtyChildrenCount  { get { if (this.IsDisruptive) { return _cantSitWith.Count; }
                else return 0;
            }
        }
    
        public List<Child> CantSitWith { get { return _cantSitWith; } set { _cantSitWith = value; } }


        private List<Child> _cantSitWith;

        public Child()
        {
        
        }
        public Child(string name)
        {
            Name = name;
            this.Id = System.Threading.Interlocked.Increment(ref m_Counter);
          
        }


        public Child(string name, List<Child> cantSitWith)
        {
            IsDisruptive = true;
            this.CantSitWith = cantSitWith;
            this.Id = System.Threading.Interlocked.Increment(ref m_Counter);

        }

        public void MakeNotDisruptive()
        {
            IsDisruptive = false;
            if(this.CantSitWith != null)
            this.CantSitWith.Clear();

        }
    
        public void MakeDisruptive(List<Child> cantSitWith)
        {
            
            IsDisruptive = true;
            this.CantSitWith = cantSitWith;
         
        }
    
        public override string ToString()
        {
            return Name;
        }

    }
}
