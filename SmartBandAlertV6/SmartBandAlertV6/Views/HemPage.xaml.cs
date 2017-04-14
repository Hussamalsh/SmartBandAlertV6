using SmartBandAlertV6.Data;
using SmartBandAlertV6.Messages;
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
    public partial class HemPage : ContentPage
    {

        public BLEProfileManager BLEProfileManager;



        public HemPage()
        {
            BLEProfileManager = App.BLEProfileManager;

            InitializeComponent();


            theBTunits.IsPullToRefreshEnabled = true;

            stopDanger.BindingContext = new { w1 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc2 = Color.White };
            startDanger.BindingContext = new { w0 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc1 = Color.FromHex("#ededed") };

            //Battery 
            progBar.BindingContext = new { w4 = App.ScreenWidth * 160 / (App.ScreenDPI * 3), theprog = 0.5 };
            progBar.Scale = 1;
            batterystack.HorizontalOptions = LayoutOptions.CenterAndExpand;
            progBarText.BindingContext = new { theprogtext = "50%" };
            checkBattery.BindingContext = new { bgc3 = Color.White };



            startDanger.Clicked += (s, e) => {

                if (BLEProfileManager.bleprofile.Devices.Count != 0)
                {
                    var device = BLEProfileManager.bleprofile.Devices.FirstOrDefault().Device;
                    string status = device.State.ToString();
                    if (status.Equals("Disconnected"))
                    {
                        DisplayAlert("No BlueTooth ", "You need to connect to SmartBand Alert first", "Ok");

                    }
                    else
                    {
                        var message = new StartLongRunningTaskMessage();
                        MessagingCenter.Send(message, "StartLongRunningTaskMessage");
                    }

                }
                else
                    DisplayAlert("No BlueTooth ", "You need to connect to SmartBand Alert first", "Ok");


            };

            stopDanger.Clicked += (s, e) => {
                var message = new StopLongRunningTaskMessage();
                MessagingCenter.Send(message, "StopLongRunningTaskMessage");
            };


        }

        public void Button_OnClickedScanToggle(object sender, EventArgs e)
        {

        }

        async void Button_OnClickedBatteriUppdat(Object obj, EventArgs e)
        {

        }

        async void connectClicked(object sender, EventArgs e)
        {

        }


    }
}
