using ClassRoomPlanner.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassRoomPlanner.ViewModels
{

  

    public class ChildrenDataService
    { 

        public ObservableCollection<Child> ChildrenInClass { get; set; }

        public async Task SaveChildrenAsync(ObservableCollection<Child> childrenInClass)
        {

            var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Child>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                          "childrenCollection",
                          CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, childrenInClass);
            }
        }


        public async Task<ObservableCollection<Child>> LoadChildrenAsync()
        {

            var file = ApplicationData.Current.LocalFolder.TryGetItemAsync("childrenCollection");
            if (file != null)
            {


                using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("childrenCollection"))
                {
                    DataContractJsonSerializer childSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Child>));
                    ChildrenInClass = (ObservableCollection<Child>)childSerializer.ReadObject(myStream);

                }

            }
            else
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("childrenCollection");
            }

                return ChildrenInClass;

        }
    }
}
        
    
