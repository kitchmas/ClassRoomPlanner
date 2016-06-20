using ClassRoomPlanner.Model;
using ClassRoomPlanner.Seating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using ClassRoomPlanner.ViewModels;

namespace ClassRoomPlanner.ViewModels
{
    public class SeatingPlanViewModel : ClassRoomViewModelBase
    {


        private SeatPlanner SeatPlanner;

        private ObservableCollection<Child> childrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> ChildrenInClass
        {
            get { return childrenInClass; }
        }

        private ObservableCollection<Table<Child>> tablesInClass = new ObservableCollection<Table<Child>>();
        public ObservableCollection<Table<Child>> TablesInClass
        {
            get { return tablesInClass; }
        }

        public SeatingPlanViewModel()
        {
           
        }


        public void GenerateRandomClass() { SeatPlanner.RandomSeating(); }
        
       public void GenerateSeperateSeats() {tablesInClass = SeatPlanner.DisruptiveCantSitBySeat(); }
       public void GenerateSeperateTables() {tablesInClass = SeatPlanner.DisruptiveCantSitByTable(); }


        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            await TableDataService.SaveTablesAsync(TablesInClass);
            await base.OnNavigatedFromAsync(pageState, suspending);
        }



        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            ObservableCollection<Table<Child>> tables = await TableDataService.LoadTablesAsync();
            childrenInClass = await ChildrenDataService.LoadChildrenAsync();
            await base.OnNavigatedToAsync(parameter, mode, state);
            UpdateTables(tables);
            SeatPlanner = new SeatPlanner(tablesInClass, childrenInClass);
            SeatPlanner.RandomSeating();

        }



        

        private void UpdateTables(ObservableCollection<Table<Child>> tables)
        {
  
            foreach (var table in tables)
            {
                
                TablesInClass.Add(table);
            }
        }
    }
}
