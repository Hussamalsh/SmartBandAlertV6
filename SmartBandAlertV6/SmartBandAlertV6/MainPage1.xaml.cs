using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBandAlertV6.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SmartBandAlertV6.Pages;

namespace SmartBandAlertV6
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage1 : MasterDetailPage
    {

        public List<MasterPageItem> menuList { get; set; }

        public MainPage1()
        {

            InitializeComponent();

            menuList = new List<MasterPageItem>();

            // Creating pages for menu navigation
            var Hem = new MasterPageItem() { Title = "Hem", IconSource = "hem.png", TargetType = typeof(Hem) };
            var Profil = new MasterPageItem() { Title = "Profil", IconSource = "profil.png", TargetType = typeof(Profil) };
            var Beskyddare = new MasterPageItem() { Title = "Beskyddare", IconSource = "beskyddare.png", TargetType = typeof(Beskyddare) };
            //var Installningar = new MasterPageItem() { Title = "Inställningar", Icon = "installningar.png", TargetType = typeof(Installningar) };
            var TestAlarm = new MasterPageItem() { Title = "TestAlarm", IconSource = "hjalp.png", TargetType = typeof(GPStestPage) };

            var Hjalp = new MasterPageItem() { Title = "Hjälp", IconSource = "hjalp.png", TargetType = typeof(Hjalp) };
            var Om = new MasterPageItem() { Title = "Om", IconSource = "om.png", TargetType = typeof(Om) };

            var Logout = new MasterPageItem() { Title = "LogOut", IconSource = "om.png", TargetType = typeof(LogoutPage) };



            // Adding menu items to menuList
            menuList.Add(Hem);
            menuList.Add(Profil);
            menuList.Add(Beskyddare);
            //menuList.Add(Installningar);
            menuList.Add(TestAlarm);
            menuList.Add(Hjalp);
            menuList.Add(Om);
            menuList.Add(Logout);


            imgSRC.Source = App.ProfilePic;
            nameSet.Text = App.FacebookName;

            // Setting our list to be ItemSource for ListView in MainPage.xaml
            navigationDrawerList.ItemsSource = menuList;

            // Initial navigation(homepage)
            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(Hem)));
        }

        // Event for Menu Item selection, here we are going to handle navigation based
        // on user selection in menu ListView
        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            var item = (MasterPageItem)e.SelectedItem;
            Type page = item.TargetType;

            Detail = new NavigationPage((Page)Activator.CreateInstance(page));
            IsPresented = false;
        }
    }
}
