using ClassRoomPlanner.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace ClassRoomPlanner.Services
{
    public class TableDataService
    {
        public ObservableCollection<Table<Child>> TablesInClass;
      


        public async Task SaveTablesAsync(ObservableCollection<Table<Child>> tablesInClass)
        {

            var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Table<Child>>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                          "tablesCollection",
                          CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, tablesInClass);
            }

        }


        public async Task<ObservableCollection<Table<Child>>> LoadTablesAsync()
        {

            var file = ApplicationData.Current.LocalFolder.TryGetItemAsync("tablesCollection");
            if (file != null)
            {
                
                    using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("tablesCollection"))
                    {
                    try
                    {
                        DataContractJsonSerializer tableSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Table<Child>>));
                        TablesInClass = (ObservableCollection<Table<Child>>)tableSerializer.ReadObject(myStream);
                    }
                    catch (JsonReaderException e)
                    {
                        MessageDialog t = new MessageDialog(e.ToString());
                        await t.ShowAsync();
                    }
                }
                }
                
            

            else
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("tablesCollection");
            }


            return TablesInClass;
        }


    }
}
