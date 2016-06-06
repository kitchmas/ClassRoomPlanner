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

namespace ClassRoomPlanner.ViewModels
{
   public class EditChildViewModel : ViewModelBase
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

    


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            await LoadChildrenAsync();
            //Move this next line to somewhere more approperitate
           
            await base.OnNavigatedToAsync(parameter, mode, state);
        }



    }
}
