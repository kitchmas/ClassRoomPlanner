﻿using System;
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
using ClassRoomPlanner.ViewModels;


namespace ClassRoomPlanner.ViewModels
{
    public class AddChildViewModel : ClassRoomViewModelBase
    {
        public AddChildViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            
            }
        }

        private ObservableCollection<Child> childrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> ChildrenInClass
        {
            get { return childrenInClass; }
            set {
                childrenInClass = value;
 }
        }

        private Child selectedChild; 
        public Child SelectedChild
        {
            get { return selectedChild; }
            set
            {
                if (value == null)
                    return;
 

                else
                {
                    selectedChild = value;
                
                    base.RaisePropertyChanged();
                
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

   
        public void DeleteChild(int Id)
        {
          
            var childMatch = from child in ChildrenInClass
                             where child.Id == Id
                             select child;

            var childToRemove = childMatch.First();
            if (childToRemove != null)
                ChildrenInClass.Remove(childToRemove);
        }






      


        
    

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            await ChildrenDataService.SaveChildrenAsync(ChildrenInClass);
            await base.OnNavigatedFromAsync(pageState, suspending);

        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            // child is being returned as null
            ObservableCollection<Child> children = await ChildrenDataService.LoadChildrenAsync();

            await base.OnNavigatedToAsync(parameter, mode, state);
            UpdateChildrenInClass(children);
        }

        private void UpdateChildrenInClass(ObservableCollection<Child> children)
        {
            if(children != null)
            foreach (var child in children)
            {
                ChildrenInClass.Add(child);
            }
        }

        private DelegateCommand<int> deleteCommand;
        public DelegateCommand<int> DeleteCommand
            => deleteCommand ?? (deleteCommand = new DelegateCommand<int>(DeleteChild, CandDeleteChild));


        public void GoToAddTablesPage() => NavigationService.Navigate(typeof(Views.AddTablesPage));

    }
}
