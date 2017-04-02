using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage

    {

        public MainPage()

        {

            InitializeComponent();



            masterPage.ListView.ItemSelected += OnItemSelected;



            if (Device.OS == TargetPlatform.Windows)
            {

                Master.Icon = "todo.png";

            }

        }



        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)

        {

            var item = e.SelectedItem as MasterPageItem;

            if (item != null)
            {

                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));

                masterPage.ListView.SelectedItem = null;

                IsPresented = false;

            }

        }

    }
}
