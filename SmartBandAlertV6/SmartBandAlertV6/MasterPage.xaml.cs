using SmartBandAlertV6.Models;
using SmartBandAlertV6.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterPage()

        {

            InitializeComponent();



            var masterPageItems = new List<MasterPageItem>();

            masterPageItems.Add(new MasterPageItem
            {

                Title = "Home Page",

                IconSource = "hem.png",

                TargetType = typeof(HomePage)

            });

            masterPageItems.Add(new MasterPageItem
            {

                Title = "FriendsList",

                IconSource = "beskyddare.png",

                TargetType = typeof(SkyddarePage)

            });

            masterPageItems.Add(new MasterPageItem
            {

                Title = "Profile",

                IconSource = "profil.png",

                TargetType = typeof(ProfilePage)

            });

            masterPageItems.Add(new MasterPageItem
            {

                Title = "GPSTest",

                IconSource = "reminders.png",

                TargetType = typeof(GPStestPage)

            });

            masterPageItems.Add(new MasterPageItem
            {

                Title = "Logout",

                IconSource = "reminders.png",

                TargetType = typeof(LogoutPage)

            });



            listView.ItemsSource = masterPageItems;

        }
    }
}
