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
     
        private static int IdCount = 0;
     
        public string Name { get; set; }
       
        public int Id { get; set; }
       
        
        
        public Boolean IsDisruptive { get; set; }

 
   
        public List<Child> cantSitWith { get; set; }

        public Child() { }

        public Child(string name)
        {
            Name = name;
            IdCount++;
            Id = IdCount;
        }

        public Child(string name, List<Child> cantSitWith)
        {
            IsDisruptive = true;
            this.cantSitWith = cantSitWith;
       

        }
    
        public void MarkAsNaughty(List<Child> cantSitWith)
        {
            
            IsDisruptive = true;
            this.cantSitWith = cantSitWith;
         
        }
    
        public override string ToString()
        {
            return Name;
        }

    }
}
