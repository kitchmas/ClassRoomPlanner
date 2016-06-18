using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using WindowsApp1.Services;

namespace WindowsApp1.ViewModels
{
    //Summary : Contains The ClassRoomData to be used across all viewModels
    public class ClassRoomViewModelBase : ViewModelBase
    {


        

        public ChildrenDataService ChildrenDataService { get; set; } = new ChildrenDataService();
        public TableDataService TableDataService { get; set; } = new TableDataService();

    }
}
