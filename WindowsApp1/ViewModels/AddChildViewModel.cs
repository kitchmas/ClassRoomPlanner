using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Template10.Mvvm;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Navigation;
using ClassRoomPlanner.Model;
using System.Diagnostics;

namespace ClassRoomPlanner.ViewModels
{
    public class AddChildViewModel : ViewModelBase
    {
        private ObservableCollection<Child> childrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> ChildrenInClass
        {
            get { return childrenInClass; }
        }

        private Child selectedChild; //used to add clicked children too SelectedChildren.
        public Child SelectedChild
        {
            get { return selectedChild; }
            set
            {
                if (value == null)
                    return;
                //if (SelectedChildren.Any(item => item.Id == value.Id) == true)
                //{
                //    SelectedChildren.Remove(value);
                //    UpdateChildCommand.FireCanExecuteChanged();
                //}

                else
                {
                    selectedChild = value;
                    //SelectedChildren.Add(_selectedChild);
                    base.RaisePropertyChanged();
                    //UpdateChildCommand.FireCanExecuteChanged();
                }
            }
        }

        private string newChildName;
        public string NewChildName
        {
            get { return newChildName; }
            set
            {
                newChildName = value;
                base.RaisePropertyChanged();

            }
        }





        public void AddChild()
        {
            ChildrenInClass.Add(new Child(NewChildName));
            NewChildName = "";
        }


        private bool CandDeleteChild(int arg)
        {
            return true;
        }

        //Delete child from ChildrenInClass
        public void DeleteChild(int Id)
        {
            //var selectedChildrenMatch = from child in SelectedChildren
            //                            where child.Id == Id
            //                            select child;

            //if (selectedChildrenMatch.Count() > 0)
            //{
            //    var scToRemove = selectedChildrenMatch.First();
            //    if (scToRemove != null)
            //        SelectedChildren.Remove(scToRemove);
            //    return;
            //}

            var childMatch = from child in ChildrenInClass
                             where child.Id == Id
                             select child;

            var childToRemove = childMatch.First();
            if (childToRemove != null)
                ChildrenInClass.Remove(childToRemove);
        }





        private async Task SaveChildrenAsync()
        {



            var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Child>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                          "childrenCollection",
                          CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, ChildrenInClass);
            }

            //StorageFile savedCollection = await ApplicationData.Current.LocalFolder.CreateFileAsync("childrenCollection", CreationCollisionOption.ReplaceExisting);

            //try
            //{
            //    using (Stream ws = await savedCollection.OpenStreamForWriteAsync())
            //    {
            //        DataContractSerializer ChildColleictionSerializer = new DataContractSerializer(typeof(ObservableCollection<Child>));
            //        ChildColleictionSerializer.WriteObject(ws, ChildrenInClass);
            //        await ws.FlushAsync();
            //        ws.Dispose();

            //    }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Error Serializing", e);
            //}


        }

        public async Task LoadChildrenAsync()
        {

            //Update this file check to be more thorough 
            var a = ApplicationData.Current.LocalFolder.TryGetItemAsync("childrenCollection");
            if (a != null)
            {

                try
                {
                    using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("childrenCollection"))
                        if (myStream != null)
                        {
                            DataContractJsonSerializer childSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Child>));
                            var childrenCollection = (ObservableCollection<Child>)childSerializer.ReadObject(myStream);

                            foreach (var child in childrenCollection)
                            {
                                ChildrenInClass.Add(child);

                            }
                        }
                }
                catch (FileNotFoundException e)
                {
                    Debug.WriteLine(e.ToString());

                }
            }
            else
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("childrenCollection");
            }


        
    


            //using (var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("childrenCollection"))
            //{

            //    if (readStream == null)
            //    {
            //        return;
            //    }

            //    DataContractSerializer childSerializer = new DataContractSerializer(typeof(ObservableCollection<Child>));
            //    var childrenCollecton = (ObservableCollection<Child>)childSerializer.ReadObject(readStream);
            //    foreach (var child in childrenCollecton)
            //    {
            //        ChildrenInClass.Add(child);
            //    }
            //}

        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            await SaveChildrenAsync();
            await base.OnNavigatedFromAsync(pageState, suspending);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            await LoadChildrenAsync();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        private DelegateCommand<int> deleteCommand;
        public DelegateCommand<int> DeleteCommand
            => deleteCommand ?? (deleteCommand = new DelegateCommand<int>(DeleteChild, CandDeleteChild));


        public void GoToAddTablesPage() => NavigationService.Navigate(typeof(Views.AddTablesPage));

    }
}
