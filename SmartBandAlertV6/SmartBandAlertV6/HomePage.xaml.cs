using Acr.UserDialogs;
using Plugin.BLE.Abstractions;
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

namespace SmartBandAlertV6
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public BLEProfileManager BLEProfileManager;
        public HomePage()
        {
            BLEProfileManager = App.BLEProfileManager;
            InitializeComponent();

            longRunningTask.Clicked += (s, e) => {

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

                }else
                    DisplayAlert("No BlueTooth ", "You need to connect to SmartBand Alert first", "Ok");


            };

            stopLongRunningTask.Clicked += (s, e) => {
                var message = new StopLongRunningTaskMessage();
                MessagingCenter.Send(message, "StopLongRunningTaskMessage");
            };

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OnAlertYesNoClicked();

            //GetSystemConnectedOrPairedDevices();
            if (BLEProfileManager.bleprofile.Devices.Count != 0)
                this.Devicesl.ItemsSource = BLEProfileManager.bleprofile.Devices;

        }

        private readonly IUserDialogs _userDialogs;

        IDisposable scan;
        //public bool IsScanning { get; private set; }
        bool firsttime = true;

        async void Button_OnClickedScanToggle(Object obj, EventArgs e)
        {
            if (firsttime && BLEProfileManager.bleprofile.Devices.Count == 0)
            {
                BLEProfileManager.init();
                firsttime = false;
            }


            Task.Delay(3000);
            if (BLEProfileManager.bleprofile.IsScanning)
            {
                this.scan?.Dispose();
                //BLEProfileManager.bleprofile.Adapter.StopScanningForDevicesAsync();
                this.ScanText.Text = BLEProfileManager.bleprofile.Adapter.IsScanning ? "Stop Scan" : "Scan";
                //this.Devicesl.ItemsSource = BLEProfileManager.bleprofile.Devices;
                var counterscan = 0;
                while (BLEProfileManager.bleprofile.IsScanning)
                {
                    //DisplayAlert("Question?", "Do you have Smartband Alert product?", "Yes", "No");
                     var answer = await DisplayAlert("Wait Please", "Looking for SmartBand Alert", "Ok", ""+counterscan++);
                   // _userDialogs.ShowLoading($"Disconnect in {counterscan++}s...", MaskType.Black);

                    if (BLEProfileManager.bleprofile.Devices.Count != 0)
                    {
                        this.Devicesl.ItemsSource = BLEProfileManager.bleprofile.Devices;
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
                    this.Devicesl.ItemsSource = BLEProfileManager.bleprofile.Devices;

                    /*string s = BLEProfileManager.bleprofile.Devices.FirstOrDefault().Device.State.ToString();
                    if (s.Equals("Disconnected"))
                    {
                        BLEProfileManager.bleprofile.Adapter.ConnectToDeviceAsync(BLEProfileManager.bleprofile.Devices.FirstOrDefault().Device);
                    }
                    else
                        this.SyststateL.Text = "Connected: " + BLEProfileManager.bleprofile.Devices.FirstOrDefault().IsConnected.ToString();*/
                }
                else
                {
                    this.SyststateL.Text = "Connected: Couldt Find";
                    BLEProfileManager.init();
                }

            }
        }



        void SelectDevice(object sender, SelectedItemChangedEventArgs e)
        {

        }


        async void ConnectAndDisposeDevice(object o, EventArgs e)
        {


            var mi = ((MenuItem)o);
            DeviceListItemViewModel item = mi.CommandParameter as DeviceListItemViewModel;


            try
            {

                //await Adapter.ConnectToDeviceAsync(Devices.FirstOrDefault().Device);
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

            this.Devicesl.ItemsSource = null;
            DisplayAlert("Your connected to ", item.Device.Name, "Ok");

            //BLEProfileManager.ScanForDevices();

            App.BlegUID = item.Device.Id.ToString();
            ((App)App.Current).SaveProfile();

            this.Systconnocted.Text = "Device name: " + BLEProfileManager.bleprofile.Devices.FirstOrDefault().Device.Name;
            this.SyststateL.Text = "Connected: " + BLEProfileManager.bleprofile.Devices.FirstOrDefault().IsConnected.ToString();

            this.Devicesl.ItemsSource = BLEProfileManager.bleprofile.Devices;


        }

        private void GetSystemConnectedOrPairedDevices()
        {
            try
            {
                //heart rate
                //var guid = Guid.Parse("00000000-0000-0000-0000-e81af8931c9f");
                if (!String.IsNullOrEmpty(App.BlegUID))
                {


                    var guid = Guid.Parse(App.BlegUID);

                    BLEProfileManager.bleprofile.SystemDevices = BLEProfileManager.bleprofile.Adapter.GetSystemConnectedOrPairedDevices(new[] { guid }).Select(d => new DeviceListItemViewModel(d)).ToList();
                    //RaisePropertyChanged(() => SystemDevices);
                    //SysDevicesX.ItemsSource = SystemDevices;
                    this.Systconnocted.Text = "Device name: " + BLEProfileManager.bleprofile.SystemDevices.FirstOrDefault().Device.Name;
                    BLEProfileManager.bleprofile.Adapter.ConnectToDeviceAsync(BLEProfileManager.bleprofile.SystemDevices.FirstOrDefault().Device);
                    this.SyststateL.Text = "Connected: " + BLEProfileManager.bleprofile.SystemDevices.FirstOrDefault().IsConnected.ToString();
                }
                else
                {
                    this.Systconnocted.Text = "Device name: Couldnt find a device";
                    this.SyststateL.Text = "Connected: ";
                }
            }
            catch (Exception ex)
            {
               // Trace.Message("Failed to retreive system connected devices. {0}", ex.Message);
            }
        }


        async void OnAlertYesNoClicked()
        {
            if (App.HaveSmartBand == false)//ToFix= see if the user answer this quetion or not
            {
                var answer = await DisplayAlert("Question?", "Do you have Smartband Alert product?", "Yes", "No");
                Debug.WriteLine("Answer: " + answer);
                App.HaveSmartBand = answer;
                ((App)App.Current).SaveProfile();
            }

        }



        


    }
}
