using Acr.UserDialogs;
using Plugin.BluetoothLE;
using SmartBandAlertV6.Data;
using SmartBandAlertV6.Messages;
using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HemPage : ContentPage
    {
        IDisposable scan;


        //public IAppState AppState;

       // public string ScanText { get; private set; }
        public bool IsScanning { get; private set; }

        //public string Title { get; private set; }

        IDisposable subNoTIFY;


        public BLEAcrProfileManager bleACRProfileManager;
        public bool dangerModeOn = false;
        public HemPage()
        {
            
            InitializeComponent();

            bleACRProfileManager = App.BLEAcrProfileManager;

            //theBTunits.IsPullToRefreshEnabled = true;

            stopDanger.BindingContext = new { w1 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc2 = Color.White };
            startDanger.BindingContext = new { w0 = App.ScreenWidth * 160 / (App.ScreenDPI), bgc1 = Color.FromHex("#ededed") };

            //Battery 
            progBar.BindingContext = new { w4 = App.ScreenWidth * 160 / (App.ScreenDPI * 3), theprog = 0.5 };
            progBar.Scale = 1;
            batterystack.HorizontalOptions = LayoutOptions.CenterAndExpand;
            progBarText.BindingContext = new { theprogtext = "50%" };
            checkBattery.BindingContext = new { bgc3 = Color.White };

            startDanger.Clicked += async (s, e) =>
            {

                if (App.isConnectedBLE)
                {
                    //subNoTIFY.Dispose();
                    dangerModeOn = true;
                    var message = new StartLongRunningTaskMessage();
                    MessagingCenter.Send(message, "StartLongRunningTaskMessage");
                }
                else
                    await DisplayAlert("Wrong ", "Conect to a device first", "Ok");
            };

            stopDanger.Clicked += (s, e) => {
                dangerModeOn = false;
                var message = new StopLongRunningTaskMessage();
                MessagingCenter.Send(message, "StopLongRunningTaskMessage");
            };

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if(bleACRProfileManager.bleprofile.BleAdapter.Status == AdapterStatus.PoweredOn)
            {

                if (bleACRProfileManager.bleprofile.Devices.Count ==0)
                {
                    bleACRProfileManager.intit();
                    IDevice dev = CrossBleAdapter.Current.GetConnectedDevices().FirstOrDefault();
                    if(dev!= null)
                    {
                        bleACRProfileManager.bleprofile.Devices.Add(new ScanResultViewModel());
                        bleACRProfileManager.bleprofile.Devices.FirstOrDefault().TrySetCustom(dev);
                        this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;

                    }
                }
                else
                {
                    this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;
                }

                //111
                bleACRProfileManager.bleprofile.BleAdapter.WhenScanningStatusChanged().Subscribe(on =>
                {
                    this.IsScanning = on;
                    this.ScanText.Text = on ? "Stop Scan" : "Scan";
                });
            }else
                await DisplayAlert("Error: Bluetooth is off?", "Turn on the Bluetooth?", "OK");
        }

        public async void Button_OnClickedScanToggle(object sender, EventArgs e)
        {
            if (bleACRProfileManager.bleprofile.BleAdapter.Status == AdapterStatus.PoweredOn)
            {
                if (this.IsScanning)
                {
                    this.scan?.Dispose();
                }
                else
                {
                    bleACRProfileManager.bleprofile.Devices.Clear();
                    this.ScanText.Text = "Stop Scan";

                    this.scan = bleACRProfileManager.bleprofile.BleAdapter
                        .Scan()
                        .Subscribe(this.OnScanResult);
                }
            }
            else
                await DisplayAlert("Error: Bluetooth is off?", "Turn on the Bluetooth?", "OK");

        }

        void OnScanResult(IScanResult result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var dev = bleACRProfileManager.bleprofile.Devices.FirstOrDefault(x => x.Uuid.Equals(result.Device.Uuid));
                if (dev != null && !String.IsNullOrEmpty(dev.Name))
                {
                    dev.TrySet(result);
                }
                else
                {
                    dev = new ScanResultViewModel();
                    dev.TrySet(result);
                    if(!String.IsNullOrEmpty(dev.Name))
                        bleACRProfileManager.bleprofile.Devices.Add(dev);
                }
            });
            this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;

        }
        private bool dotwice = false;
        async void Button_OnClickedBatteriUppdat(Object obj, EventArgs e)
        {
            if (App.isConnectedBLE)
            {
                try
                {
                    byte[] toBytes = Encoding.UTF8.GetBytes("11");
                     bleACRProfileManager.bleprofile.CharacteristicWrite.WriteWithoutResponse(toBytes);
                    if(!dotwice)
                    {
                        bleACRProfileManager.bleprofile.CharacteristicWrite.WriteWithoutResponse(toBytes);
                        dotwice = true;
                    }
                }
                catch (Exception ex)
                {
                    UserDialogs.Instance.Alert(ex.ToString());
                }
            }
            else
                await DisplayAlert("Error:", "Connect to a device first", "OK");

        }

        IDevice device;

        async void connectBClicked(object sender, EventArgs e)
        {
            bool answer;
            this.scan?.Dispose();

            var button = sender as Button;
            ScanResultViewModel item = button.BindingContext as ScanResultViewModel;
            device = item.Device;

            bleACRProfileManager.cleanEverything(device);
            try
            {
                // don't cleanup connection - force user to d/c
                if (this.device.Status == ConnectionStatus.Disconnected)
                {
                    answer = await DisplayAlert("Beskyddare", "Vill du ansluta till den här bluetooth enheten?", "Ja", "Nej");
                    if (answer == true)
                    {
                        using (var cancelSrc = new CancellationTokenSource())
                        {
                            using (UserDialogs.Instance.Loading("Connecting", cancelSrc.Cancel, "Cancel"))
                            {
                                await this.device.Connect().ToTask(cancelSrc.Token);
                                App.isConnectedBLE = true;
                            }
                        }

                        this.device.WhenAnyCharacteristicDiscovered().Subscribe(characteristic =>
                        {

                            if (characteristic.Uuid.Equals(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e")))
                            {
                                bleACRProfileManager.bleprofile.CharacteristicRead = characteristic;
                                // once you have your characteristic instance from the service discovery
                                // this will enable the subscriptions to notifications as well as actually hook to the event
                                subNoTIFY = bleACRProfileManager.bleprofile.CharacteristicRead.SubscribeToNotifications().Subscribe(result =>
                                { result.Characteristic.SubscribeToNotifications().Subscribe(x => this.SetReadValue(x, true)); });

                            }
                            if (characteristic.Uuid.Equals(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e")))
                            {
                                bleACRProfileManager.bleprofile.CharacteristicWrite = characteristic;
                            }


                        });

                    }

                }
                else
                {
                    answer = await DisplayAlert("Beskyddare", "Vill du avsluta till den här bluetooth enheten?", "Ja", "Nej");
                    if (answer == true)
                    {
                        this.device.CancelConnection();
                        App.isConnectedBLE = false;
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.ToString());
            }
            //Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).UpdateD();
            this.theBTunits.ItemsSource = bleACRProfileManager.bleprofile.Devices;
            

        }

        public string Value { get; set; }

        void SetReadValue(CharacteristicResult result, bool fromUtf8)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //this.IsValueAvailable = true;
                //this.LastValue = DateTime.Now;

                if (result.Data == null)
                    this.Value = "EMPTY";
                else
                {

                    string tempval = "emp";
                    if (String.IsNullOrEmpty(Value))
                    {
                        this.Value = System.Text.Encoding.UTF8.GetString(result.Data, 0, result.Data.Length-1);
                    }
                    else
                    {
                        tempval = System.Text.Encoding.UTF8.GetString(result.Data, 0, result.Data.Length-1);

                    }


                    if (!Value.Equals(tempval) && result.Data.Length<5)
                    {
                        if(!tempval.Equals("emp"))
                            this.Value = tempval;
                            int nr = int.Parse(Value);
                            if (nr > 107)
                                await DisplayAlert("Charging:", " The Battery is on Charge", "OK");
                            else
                            {
                                double resultlvl = (((double)nr / 107) * 100);
                                progBar.BindingContext = new { w4 = App.ScreenWidth * 160 / (App.ScreenDPI * 3), theprog = (resultlvl / 100) };
                                progBarText.BindingContext = new { theprogtext = resultlvl.ToString("#") + "%" };
                            }
                        }else if(dangerModeOn && result.Data.Length > 5)
                        {
                            postAlarm();
                        }

                        



                    }
                //this.Value = fromUtf8 ? Encoding.UTF8.GetString(result.Data, 0, result.Data.Length) : BitConverter.ToString(result.Data);

            });
        }
        public bool postonce = false;
        public void postAlarm()
        {
            if(!postonce)
            {
                Debug.WriteLine("Doooooooooo Pooooooooooooost");

                Device.StartTimer(TimeSpan.FromSeconds(5), () =>
                {
                    // Do something
                    postonce = false;
                    return false; // True = Repeat again, False = Stop the timer
                });
                postonce = true;
            }
        }

        public void starttime() {

           

        }


    }
}
