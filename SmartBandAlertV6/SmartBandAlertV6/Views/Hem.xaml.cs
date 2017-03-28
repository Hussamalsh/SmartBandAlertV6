using SmartBandAlertV6.Data;
using SmartBandAlertV6.Messages;
using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Hem : ContentPage
    {

        public BLEProfileManager BLEProfileManager;



        public Hem()
        {
            BLEProfileManager = App.BLEProfileManager;

            InitializeComponent();


            theBTunits.IsPullToRefreshEnabled = true;

            stopDanger.BindingContext = new { w1 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc2 = Color.White };
            startDanger.BindingContext = new { w0 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc1 = Color.FromHex("#ededed") };




            startDanger.Clicked += (s, e) => {

                if (BLEProfileManager.bleprofile.Devices.Count != 0)
                {

                    string status = BLEProfileManager.bleprofile.Devices.FirstOrDefault().Device.State.ToString();
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


        protected override void OnAppearing()
        {
            base.OnAppearing();

            OnBLEYesNoClicked();

            //GetSystemConnectedOrPairedDevices();
            if (BLEProfileManager.bleprofile.Devices.Count != 0)
                this.theBTunits.ItemsSource = BLEProfileManager.bleprofile.Devices;

        }


        async void connectClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Beskyddare", "Vill du ansluta till den här bluetooth enheten?", "Ja", "Nej");

            if (answer == true) {
                //var mi = ((MenuItem)sender);
                // DeviceListItemViewModel item = mi.CommandParameter as DeviceListItemViewModel;

                var button = sender as Button;
                DeviceListItemViewModel item = button.BindingContext as DeviceListItemViewModel;

            try
            {
                BLEProfileManager.bleprofile.Adapter.ConnectToDeviceAsync(item.Device);

                item.Update();

                for (var i = 2; i >= 1; i--)
                {

                    await Task.Delay(1000);

                }

            }
            catch (Exception ex)
            {
            }
            finally
            {
                // this.Devicesl.ItemsSource = this.Devices;

            }

            this.theBTunits.ItemsSource = null;
            DisplayAlert("Your connected to ", item.Device.Name, "Ok");

            App.BlegUID = item.Device.Id.ToString();
            ((App)App.Current).SaveProfile();

            this.theBTunits.ItemsSource = BLEProfileManager.bleprofile.Devices;


            }

        }

        async void disconnectClicked(object sender, EventArgs e)
        {

        }

        bool firsttime = true;

        async void Button_OnClickedScanToggle(Object obj, EventArgs e)
        {
            /*if (firsttime && BLEProfileManager.bleprofile.Devices.Count == 0)
            {
                BLEProfileManager.init();
                firsttime = false;
            }*/

            if (BLEProfileManager.bleprofile.ble.IsOn)
            {

            Task.Delay(3000);
            if (BLEProfileManager.bleprofile.IsScanning)
            {
                //BLEProfileManager.bleprofile.Adapter.StopScanningForDevicesAsync();
                this.ScanText.Text = BLEProfileManager.bleprofile.Adapter.IsScanning ? "Stop Scan" : "Scan";
                //this.Devicesl.ItemsSource = BLEProfileManager.bleprofile.Devices;
                var counterscan = 0;
                while (BLEProfileManager.bleprofile.IsScanning)
                {
                    //DisplayAlert("Question?", "Do you have Smartband Alert product?", "Yes", "No");
                    var answer = await DisplayAlert("Wait Please", "Looking for SmartBand Alert", "Ok", "" + counterscan++);
                    // _userDialogs.ShowLoading($"Disconnect in {counterscan++}s...", MaskType.Black);

                    if (BLEProfileManager.bleprofile.Devices.Count != 0)
                    {
                        this.theBTunits.ItemsSource = BLEProfileManager.bleprofile.Devices;
                        this.ScanText.Text = BLEProfileManager.bleprofile.Adapter.IsScanning ? "Stop Scan" : "Scan";

                    }

                }

            }
            else
            {
                this.ScanText.Text = BLEProfileManager.bleprofile.Adapter.IsScanning ? "Stop Scan" : "Scan";
                //BLEProfileManager.ScanForDevices();
                if (BLEProfileManager.bleprofile.Devices.Count != 0)
                {
                    this.theBTunits.ItemsSource = BLEProfileManager.bleprofile.Devices;
                }
                else
                {
                    //this.SyststateL.Text = "Connected: Couldt Find";
                    BLEProfileManager.init();
                }

            }


            }else
                DisplayAlert("Error: Bluetooth is off?", "Turn on the Bluetooth?", "OK");
        }




        async void OnBLEYesNoClicked()
        {
            BLEProfileManager.init();
            if (!BLEProfileManager.bleprofile.ble.IsOn)//ToFix= see if the user answer this quetion or not
            {
                await DisplayAlert("Error: Bluetooth is off?", "Turn on the Bluetooth?", "OK");
            }

        }

    }
}
