using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using ClassRoomPlanner.Views;
using WindowsApp1.ViewModels;

namespace ClassRoomPlanner.ViewModels
{
    public class WelcomPageViewModel : ClassRoomViewModelBase
    
    {
        public List<string> Title { get; set; } = new List<string>() { "Ms", "Mrs", "Miss", "Mr" };




        private string _teacherTitle = "Mr";
        public string TeacherTitle
        {
            get { return _teacherTitle; }
            set
            {
                _teacherTitle = value;
                base.RaisePropertyChanged();

            }
        }

        private string teacherName;

        public string TeacherName
        {
            get { return teacherName; }
            set
            {
                teacherName = value;
                base.RaisePropertyChanged();


            }
        }

        public void GoToAddChildrenPage()
        {
         
            
            NavigationService.Navigate(typeof(AddChildrenPage));
                }


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.Any())
            {
                if (state.ContainsKey("TeacherName"))
                {
                    TeacherName = state["TeacherName"] as string;
                }
            }
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            if (suspending)
            {
                pageState["TeacherName"] = TeacherName;

            }
            pageState["TeacherName"] = TeacherName;
            return base.OnNavigatedFromAsync(pageState, suspending);
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            
            return base.OnNavigatingFromAsync(args);
        }
    }
}
