using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WindowsApp1.ViewModels;

namespace ClassRoomPlanner.ViewModels
{
   public class EditChildViewModel : ClassRoomViewModelBase
    {

        private ObservableCollection<Child> childrenInClass = new ObservableCollection<Child>();
        public ObservableCollection<Child> ChildrenInClass
        {
            get { return childrenInClass; }
        }

        private Child selectedChild;
        public Child SelectedChild
        {
            get { return selectedChild; }
            set
            {
                selectedChild = value;


                base.RaisePropertyChanged();
            }
        }
        public void GoToEditDistractingChildrenPage() => NavigationService.Navigate(typeof(Views.EditDistractingChildrenPage),SelectedChild);


        public void GoToSeatPlanView() => NavigationService.Navigate(typeof(Views.SeatingPlanView));

     




    


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            childrenInClass = await ChildrenDataService.LoadChildrenAsync();
            //Move this next line to somewhere more approperitate
           
            await base.OnNavigatedToAsync(parameter, mode, state);
        }



    }
}
