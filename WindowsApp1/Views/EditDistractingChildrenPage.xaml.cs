using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ClassRoomPlanner.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassRoomPlanner.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditDistractingChildrenPage : Page
    {
        public EditDistractingChildrenPage()
        {
            this.InitializeComponent();
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            Child child = ((Button)sender).DataContext as Child;
            foreach (var item in NaughtySelectionList.Items)
            {
                //Compare item to be deselected
                if (child == item)
                    NaughtySelectionList.SelectedItems.Remove(item);
            }
        }
    }
}
