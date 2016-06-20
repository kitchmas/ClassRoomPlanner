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
using ClassRoomPlanner.ViewModels;

namespace ClassRoomPlanner.ViewModels
{
    public class EditChildViewModel : ClassRoomViewModelBase
    {

        private Child currentChild = new Child("");
        public Child CurrentChild
        {
            get { return currentChild; }
            set
            {
                currentChild = value;

                base.RaisePropertyChanged();
            }

        }


        public string Name
        {
            get { return currentChild.Name; }
  
        }

        public string NewName { get; set; }
       





        private ObservableCollection<Child> childrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> ChildrenInClass
        {
            get { return childrenInClass; }
        }

        private ObservableCollection<Child> copyOfChildrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> CopyOfChildrenInClass
        {
            get
            {
                return copyOfChildrenInClass;
                
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
                foreach (var child in currentChild.CantSitWith)
                {
                    SelectedNaughtyChildren.Add(child);
                }
            }


            return;
        }






        public void ResetCopyOfChildrenInClass()
        {

            foreach (Child child in ChildrenInClass)
            {
                if (child.Id == currentChild.Id)
                    continue;
                CopyOfChildrenInClass.Add(child);

            }

        }




        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
   
         
            ObservableCollection<Child> children = await ChildrenDataService.LoadChildrenAsync();
           
      
            UpdateChildrenInClass(children);
 
            currentChild = ChildrenInClass.FirstOrDefault(c => c.Id == (int)parameter);
            LoadSelectedChildren();
            ResetCopyOfChildrenInClass();
            await base.OnNavigatedToAsync(parameter, mode, state);


        }

        private void UpdateChildrenInClass(ObservableCollection<Child> children)
        {
            if (children != null)
                foreach (var child in children)
                {
                    ChildrenInClass.Add(child);
                }
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

                UpdateNonDisruptive();
            else
            {
                UpdateDisruptive();

               
            }

            UpdateName();

            await ChildrenDataService.SaveChildrenAsync(ChildrenInClass);
            NavigationService.Navigate(typeof(Views.EditChildPage));
        }

        private void UpdateName()
        {
            var item = ChildrenInClass.FirstOrDefault(i => i.Id == CurrentChild.Id);
            if (item != null)
            {

                if (!String.IsNullOrEmpty(NewName))
                    item.Name = NewName;

            }
          
        }

        private void UpdateDisruptive()
        {
            var item = ChildrenInClass.FirstOrDefault(i => i.Id == CurrentChild.Id);
            if (item != null)
            {
                item.MakeDisruptive(SelectedNaughtyChildren.ToList<Child>());


            }
        }

        private void UpdateNonDisruptive()
        {
            var item = ChildrenInClass.FirstOrDefault(i => i.Id == CurrentChild.Id);
            if (item != null)
            {
                item.MakeNotDisruptive();


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

            var currentSelectionCollection = new ObservableCollection<Child>();

            foreach (var child in currentSelection)
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
    

