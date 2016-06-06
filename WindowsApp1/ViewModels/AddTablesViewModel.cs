﻿using System;
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

namespace ClassRoomPlanner.ViewModels
{
    public class AddTablesViewModel : ViewModelBase
    {

        private ObservableCollection<int> numberCollection = new ObservableCollection<int>{1,2,3,4,5,6,7,8,9,10,11,12,
        13,14,15,16,17,18,19,20,};
        public ObservableCollection<int> NumberCollection
        {
            get { return numberCollection; }
        }

        private int selectedNumberOfChairs = 1;
        public int SelectedNumberOfChairs
        {
            get { return selectedNumberOfChairs; }
            set {
                selectedNumberOfChairs = value;
                    base.RaisePropertyChanged(); }
        }

        private ObservableCollection<Table<Child>> tablesInClass = new ObservableCollection<Table<Child>>();
        public ObservableCollection<Table<Child>> TablesInClass
        {
            get { return tablesInClass; }
        }


        public void AddTable() 
        {
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


        private async Task SaveTablesAsync()
        {

            StorageFile savedCollection = await ApplicationData.Current.LocalFolder.CreateFileAsync("tableCollection", CreationCollisionOption.ReplaceExisting);

            try
            {
                using (Stream ws = await savedCollection.OpenStreamForWriteAsync())
                {
                    DataContractSerializer tableCollectionSerializer = new DataContractSerializer(typeof(ObservableCollection<Table<Child>>));
                    tableCollectionSerializer.WriteObject(ws, TablesInClass);
                   

                }
            }
            catch (Exception e)
            {
                throw new Exception("Error Serializing", e);
            }

        }

        public async Task LoadTablesAsync()
        {

            try {
              
                using (var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("tableCollection"))
                {

                    if (readStream == null)
                    {
                        return;
                    }

                    DataContractSerializer tableSerializer = new DataContractSerializer(typeof(ObservableCollection<Table<Child>>));

                    var tableCollecton = (ObservableCollection<Table<Child>>)tableSerializer.ReadObject(readStream);
                    foreach (var table in tableCollecton)
                    {
                        TablesInClass.Add(table);
                    }
                }
        }catch(Exception e)
            {
                // The folder has not been created yet. This only occurus when navigating to the page for the first time.
                return;
            }

        }


        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            await SaveTablesAsync();
            await base.OnNavigatedFromAsync(pageState, suspending);
        }

   

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await LoadTablesAsync();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }




    }
}