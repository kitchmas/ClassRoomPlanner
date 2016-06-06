using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using ClassRoomPlanner.Model;

namespace ClassRoomPlanner.ViewModels
{
    public class EditDistractingChildrenViewModel: ViewModelBase
    {



        private Child currentChild = new Child("");
        public Child CurrentChild
        {
            get { return currentChild; }
            set { currentChild = value;

                base.RaisePropertyChanged();
            }

        }

        
        public string Name
        {
            get { return currentChild.Name; }
            

        }





        private ObservableCollection<Child> childrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> ChildrenInClass
        {
            get { return childrenInClass; }
        }

        private ObservableCollection<Child> copyOfChildrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> CopyOfChildrenInClass
        {
            get { return copyOfChildrenInClass;

            }
        
            
        }

        private ObservableCollection<Child> selectedNaughtyChildren = new ObservableCollection<Child>();
        public ObservableCollection<Child> SelectedNaughtyChildren
        {
            get { return selectedNaughtyChildren; }
        }

        private void LoadSelectedChildren()
        {
            if (currentChild.IsDisruptive)
            {
                foreach (var child in currentChild.cantSitWith)
                {
                    SelectedNaughtyChildren.Add(child);
                }
            }
               
                
            return;
        }

        public async Task LoadChildrenAsync()
        {

            using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("childrenCollection"))

            {
                DataContractJsonSerializer childSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Child>));
                var childrenCollection = (ObservableCollection<Child>)childSerializer.ReadObject(myStream);

                foreach (var child in childrenCollection)
                {
                    ChildrenInClass.Add(child);

                }
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

        public void ResetCopyOfChildrenInClass()
        {
            foreach(Child child in ChildrenInClass)
            {
                CopyOfChildrenInClass.Add(child);
            }
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



        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            currentChild = (Child)parameter;
            LoadSelectedChildren();
            await LoadChildrenAsync();
            //Move this next line to somewhere more approperitate
            ResetCopyOfChildrenInClass();
            await base.OnNavigatedToAsync(parameter, mode, state);

           
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
       
            await base.OnNavigatingFromAsync(args);
        }

        public void Cancel()
        {
            NavigationService.Navigate(typeof(Views.EditChildPage));
        }

        public async void FinishedUpdating()
        {
            if (SelectedNaughtyChildren.Count == 0)
                return;
            else
            {
                UpdateChild();

                await SaveChildrenAsync();
                NavigationService.Navigate(typeof(Views.EditChildPage));
            }

           

        }

        private void UpdateChild()
        {
            var item = ChildrenInClass.FirstOrDefault(i => i.Name == CurrentChild.Name);
            if (item != null)
            {
                item.MarkAsNaughty(SelectedNaughtyChildren.ToList<Child>());

      
            }
        }

        private DelegateCommand<IList<Object>> selectionChangedCommand;
        public DelegateCommand<IList<Object>> SelectionChangedCommand
            => selectionChangedCommand ?? (selectionChangedCommand = new DelegateCommand<IList<object>>(AddToNaughty, CanAddToNaughty));


        private bool CanAddToNaughty(IList<object> arg)
        {
            return true;
            
        }

        private void AddToNaughty(IList<object> currentSelection)
{

            var currentSelectionCollection= new ObservableCollection<Child>();
          
            foreach (var child in currentSelection )
            {
                currentSelectionCollection.Add(child as Child);  
            }

            foreach (var child in currentSelectionCollection)
            {
                if (!SelectedNaughtyChildren.Contains(child))
                    SelectedNaughtyChildren.Add(child);
              
            }


            //Check if an item has been deselected.
            var copyOfSelectedNaughtyChildren = new ObservableCollection<Child>(SelectedNaughtyChildren);
            var deselectedItems = from child in copyOfSelectedNaughtyChildren
                                  where !currentSelectionCollection.Contains(child)
                                  select child;


            //Remove the DeselectedItem.
            foreach (var child in deselectedItems)
            {
                SelectedNaughtyChildren.Remove(child);
            }


            
        }
    }
}
