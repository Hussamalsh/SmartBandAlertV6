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

                        //switchconnectbutton = 0;
                        //App.isConnectedBLE = false;
                        //need to be implement
                        //BLEProfileManager.bleprofile.Adapter.DisconnectDeviceAsync(device);
                        //BLEProfileManager.bleprofile.Devices.Clear();
                        //BLEProfileManager.init();
                        BLEProfileManager.newBLEprofile();
                        Task.Delay(1000);
                        BLEProfileManager.bleprofile.Adapter.ConnectToDeviceAsync(device);
                        Task.Delay(1000);
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
            {
                /*
                if (App.button != null)
                {
                    var device = BLEProfileManager.bleprofile.Devices.FirstOrDefault(d => d.IsConnected == true);
                    if(device!= null)
                        App.button.Text = "Avslut";
                }
                */
                this.theBTunits.ItemsSource = BLEProfileManager.bleprofile.Devices;
            }
        }
        
        int switchconnectbutton = 0;
        async void connectClicked(object sender, EventArgs e)
        {
            bool answer;
            if (switchconnectbutton == 0)
            {
                answer = await DisplayAlert("Beskyddare", "Vill du ansluta till den här bluetooth enheten?", "Ja", "Nej");
            }
            else
            {
                answer = await DisplayAlert("Beskyddare", "Vill du avsluta till den här bluetooth enheten?", "Ja", "Nej");
            }

            if (answer == true) {
                //var mi = ((MenuItem)sender);
                // DeviceListItemViewModel item = mi.CommandParameter as DeviceListItemViewModel;

                var button = sender as Button;
                DeviceListItemViewModel item = button.BindingContext as DeviceListItemViewModel;

            try
            {
                
                    if (switchconnectbutton == 0)
                    {
                        switchconnectbutton = 1;

                        BLEProfileManager.bleprofile.Adapter.ConnectToDeviceAsync(item.Device);
            
                        item.Update();
                        DisplayAlert("Your connected to ", item.Device.Name, "Ok");
                        for (var i = 2; i >= 1; i--)
                        {
                            await Task.Delay(1000);
                        }
                       
                        //App.button.Text = "Avsluta";
                        App.isConnectedBLE = true;
                        App.BlegUID = item.Device.Id.ToString();
                        ((App)App.Current).SaveProfile();

                        BLEProfileManager.getUnknownServiceAsync();

                    }
                    else
                    {
                        //App.button.Text = "Anslut";
                        switchconnectbutton = 0;
                        App.isConnectedBLE = false;
                        //need to be implement
                        BLEProfileManager.bleprofile.Adapter.DisconnectDeviceAsync(item.Device);
                        DisplayAlert("Your disconnected from ", item.Device.Name, "Ok");
                        for (var i = 2; i >= 1; i--)
                        {
                            await Task.Delay(900);
                        }
      
                    }

                }
            catch (Exception ex)
            {
                    DisplayAlert("Wrong ", "Something bad happeed", "Ok");
            }
            finally
            {
                    //BLEProfileManager.bleprofile.Devices.FirstOrDefault().Update();
                    BLEProfileManager.bleprofile.Devices.FirstOrDefault(d => d.Device.Id == item.Id).Update();
                    this.theBTunits.ItemsSource = this.BLEProfileManager.bleprofile.Devices;
            }

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

        async void Button_OnClickedBatteriUppdat(Object obj, EventArgs e)
        {
            if (BLEProfileManager.bleprofile.Devices.Count != 0)
            {
                
                 // var device = BLEProfileManager.bleprofile.Devices.FirstOrDefault(d => d.IsConnected == true);
                if (App.isConnectedBLE)
                {
                    string bLvL = BLEProfileManager.getBatterylevelAsync();
                    int nr = int.Parse(bLvL);
                    if (nr > 105)
                        await DisplayAlert("Charging:", " The Battery is on Charge", "OK");
                    else
                    {
                        double result = (((double)nr / 105) * 100);
                        progBar.BindingContext = new { w4 = App.ScreenWidth * 160 / (App.ScreenDPI * 3), theprog = (result / 100) };
                        progBarText.BindingContext = new { theprogtext = result.ToString("#") + "%" };
                    }
                }
                else
                    DisplayAlert("Error:", "Connect to a device first", "OK");
            }
            else
                DisplayAlert("Error: Bluetooth is off?", "Turn on the Bluetooth?", "OK");
        }
        async void OnBLEYesNoClicked()
        {
            BLEProfileManager.init();
            int e;
            if (await checkBLEAsync())
                e=1;
                //BLEProfileManager.init();
        }


        private async Task<bool> checkBLEAsync()
        {
            bool isBLEon = true;
            if (!BLEProfileManager.bleprofile.ble.IsOn)//ToFix= see if the user answer this quetion or not
            {
                await DisplayAlert("Error: Bluetooth is off?", "Turn on the Bluetooth?", "OK");
                isBLEon = false;
            }
            return isBLEon;
        }

    }
}
