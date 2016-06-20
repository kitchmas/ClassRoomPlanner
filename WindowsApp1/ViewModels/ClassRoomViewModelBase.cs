using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using ClassRoomPlanner.Services;
using ClassRoomPlanner.ViewModels;

namespace ClassRoomPlanner.ViewModels
{
    //Summary : Contains The ClassRoomData to be used across all viewModels
    public class ClassRoomViewModelBase : ViewModelBase
    {


        

        public ChildrenDataService ChildrenDataService { get; set; } = new ChildrenDataService();
        public TableDataService TableDataService { get; set; } = new TableDataService();

    }
}
