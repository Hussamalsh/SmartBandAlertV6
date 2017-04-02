using SmartBandAlertV6.Models;
using SmartBandAlertV6.Pages;
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
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterPage()

        {

            InitializeComponent();

            this.Icon = "hamburger.png";

            var masterPageItems = new List<MasterPageItem>();



            masterPageItems.Add(new MasterPageItem
            {
                Title = "Hem",
                IconSource = "hem.png",
                TargetType = typeof(Hem)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Profil",
                IconSource = "profil.png",
                TargetType = typeof(Profil)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Beskyddare",
                IconSource = "beskyddare.png",
                TargetType = typeof(Beskyddare)
            });



            masterPageItems.Add(new MasterPageItem
            {
                Title = "TestAlarm",
                IconSource = "hjalp.png",
                TargetType = typeof(GPStestPage)

            });


            masterPageItems.Add(new MasterPageItem
            {
                Title = "Hjälp",
                IconSource = "hjalp.png",
                TargetType = typeof(Hjalp)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Om",
                IconSource = "om.png",
                TargetType = typeof(Om)
            });


            masterPageItems.Add(new MasterPageItem
            {
                Title = "LogOut",
                IconSource = "om.png",
                TargetType = typeof(LogoutPage)
            });

            imgSRC.Source = App.ProfilePic;
            nameSet.Text = App.FacebookName;

            listView.ItemsSource = masterPageItems;

        }
    }
}
