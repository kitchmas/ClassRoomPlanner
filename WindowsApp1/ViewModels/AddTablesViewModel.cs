using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using ClassRoomPlanner.Model;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using ClassRoomPlanner.ViewModels;

namespace ClassRoomPlanner.ViewModels
{
    public class AddTablesViewModel : ClassRoomViewModelBase
    {

        private ObservableCollection<int> numberCollection = new ObservableCollection<int>{1,2,3,4,5,6,7,8,9,10,11,12,
        13,14,15,16,17,18,19,20,};
        public ObservableCollection<int> NumberCollection
        {
            get { return numberCollection; }
        }

        private int selectedNumberOfChairs;
        public int SelectedNumberOfChairs
        {
            get { return selectedNumberOfChairs; }
            set {
                selectedNumberOfChairs = value;
                    base.RaisePropertyChanged(); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                base.RaisePropertyChanged();
            }
        }
        private ObservableCollection<Table<Child>> tablesInClass = new ObservableCollection<Table<Child>>();
        public ObservableCollection<Table<Child>> TablesInClass
        {
            get { return tablesInClass; }
        }


        public void AddTable() 
        {
            if (SelectedNumberOfChairs == 0)
                SelectedNumberOfChairs = 1;
            if(!string.IsNullOrEmpty(Name))
                TablesInClass.Add(new Table<Child>(Name,selectedNumberOfChairs));
            else
                TablesInClass.Add(new Table<Child>(selectedNumberOfChairs));
            SelectedNumberOfChairs = 1;
        }



        private DelegateCommand<int> deleteCommand;
        public DelegateCommand<int> DeleteCommand
            => deleteCommand ?? (deleteCommand = new DelegateCommand<int>(DeleteTable, CanDeleteTable));



        private bool CanDeleteTable(int arg)
        {
            return true;
        }

        private void DeleteTable(int id)
        {
             var tableMatch = from table in TablesInClass
                                       where table.Id == id
                                       select table;

            var tableToRemove = tableMatch.First();
            if (tableToRemove != null)
                TablesInClass.Remove(tableToRemove);
        }


        public void GoToEditChildPage() => NavigationService.Navigate(typeof(Views.EditChildPage));


       


        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            await TableDataService.SaveTablesAsync(TablesInClass);
            await base.OnNavigatedFromAsync(pageState, suspending);
        }

   

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            ObservableCollection<Table<Child>> tables = await TableDataService.LoadTablesAsync();

            await base.OnNavigatedToAsync(parameter, mode, state);
            UpdateTables(tables);
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
