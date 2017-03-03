using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapNavigationPage : ContentPage
    {


        public MapNavigationPage(string victimId)
        {
            InitializeComponent();

            getVictim(victimId);

        }//50.894967,4.341626




        void OnNavigateButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputEntry.Text))
            {
                var address = inputEntry.Text;
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                            new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                            new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Windows:
                    case TargetPlatform.WinPhone:
                        Device.OpenUri(
                            new Uri(string.Format("bingmaps:?where={0}", Uri.EscapeDataString(address))));
                        break;
                }
            }
        }

        public async void getVictim(string victimId)
        {

            var v1 = App.VictimManager.SearchVictimAsync(victimId);
            Victim v = v1.Result;

            if (!string.IsNullOrWhiteSpace(inputEntry.Text))
            {
                var address = inputEntry.Text;
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                            new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                            new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(v.Latitude + "," + v.Longitude))));
                        break;
                    case TargetPlatform.Windows:
                    case TargetPlatform.WinPhone:
                        Device.OpenUri(
                            new Uri(string.Format("bingmaps:?where={0}", Uri.EscapeDataString(address))));
                        break;
                }
            }
        }

    }
}
