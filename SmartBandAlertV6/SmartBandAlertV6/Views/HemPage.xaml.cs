using Acr.UserDialogs;
using Plugin.BluetoothLE;
using SmartBandAlertV6.Data;
using SmartBandAlertV6.Messages;
using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        IDisposable connect;

        public IAdapter BleAdapter;
        //public IAppState AppState;

        public ObservableCollection<ScanResultViewModel> Devices { get; }
       // public string ScanText { get; private set; }
        public bool IsScanning { get; private set; }

        public bool IsSupported { get; private set; }
        public string Title { get; private set; }

        public string ConnectText { get; private set; } = "Connect";


        public HemPage()
        {
            

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
                        var message = new StartLongRunningTaskMessage();
                        MessagingCenter.Send(message, "StartLongRunningTaskMessage");
            };

            stopDanger.Clicked += (s, e) => {
                var message = new StopLongRunningTaskMessage();
                MessagingCenter.Send(message, "StopLongRunningTaskMessage");
            };





            /////Here we goooooooooooooooooooooooooooooooo
            this.BleAdapter = CrossBleAdapter.Current;



            this.connect = this.BleAdapter
                .WhenDeviceStatusChanged()
                .Subscribe(x =>
                {
                    var vm = this.Devices.FirstOrDefault(dev => dev.Uuid.Equals(x.Uuid));
                    if (vm != null)
                        vm.IsConnected = x.Status == ConnectionStatus.Connected;
                });

            //this.AppState.WhenBackgrounding().Subscribe(_ => this.scan?.Dispose());
            this.BleAdapter.WhenScanningStatusChanged().Subscribe(on =>
            {
                this.IsScanning = on;
                this.ScanText.Text = on ? "Stop Scan" : "Scan";
            });
            this.Devices = new ObservableCollection<ScanResultViewModel>();





        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.BleAdapter.WhenStatusChanged().Subscribe(x => Device.BeginInvokeOnMainThread(() =>
            {
                this.IsSupported = x == AdapterStatus.PoweredOn;
                this.Title = $"BLE Scanner ({x})";
            }
            ));

        }

        public void Button_OnClickedScanToggle(object sender, EventArgs e)
        {
            if (this.IsScanning)
            {
                this.scan?.Dispose();
            }
            else
            {
                this.Devices.Clear();
                this.ScanText.Text = "Stop Scan";

                this.scan = this.BleAdapter
                    .Scan()
                    .Subscribe(this.OnScanResult);
            }
        }

        void OnScanResult(IScanResult result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var dev = this.Devices.FirstOrDefault(x => x.Uuid.Equals(result.Device.Uuid));
                if (dev != null && !String.IsNullOrEmpty(dev.Name))
                {
                    dev.TrySet(result);
                }
                else
                {
                    dev = new ScanResultViewModel();
                    dev.TrySet(result);
                    if(!String.IsNullOrEmpty(dev.Name))
                    this.Devices.Add(dev);
                }
            });
            this.theBTunits.ItemsSource = Devices;

        }

        async void Button_OnClickedBatteriUppdat(Object obj, EventArgs e)
        {
            try
            {
                byte[] toBytes = Encoding.UTF8.GetBytes("11");
                CharacteristicWrite.WriteWithoutResponse(toBytes);


                /*
                while (String.IsNullOrEmpty(Value))
                {
                    if (!String.IsNullOrEmpty(Value))
                    {
                        int nr = int.Parse(Value);
                        if (nr > 105)
                            await DisplayAlert("Charging:", " The Battery is on Charge", "OK");
                        else
                        {
                            double result = (((double)nr / 105) * 100);
                            progBar.BindingContext = new { w4 = App.ScreenWidth * 160 / (App.ScreenDPI * 3), theprog = (result / 100) };
                            progBarText.BindingContext = new { theprogtext = result.ToString("#") + "%" };
                        }
                    }

                    
                }
                Value = null;*/


            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.ToString());
            }
        }

        IDevice device;
        async void connectBClicked(object sender, EventArgs e)
        {
            bool answer;
            this.scan?.Dispose();

            var button = sender as Button;
            ScanResultViewModel item = button.BindingContext as ScanResultViewModel;
            device = item.Device;

            cleanEverything();
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
                            }
                        }
                    }

                }
                else
                {
                    answer = await DisplayAlert("Beskyddare", "Vill du avsluta till den här bluetooth enheten?", "Ja", "Nej");
                    if (answer == true)
                    {
                        this.device.CancelConnection();
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.ToString());
            }
            //Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).UpdateD();
            this.theBTunits.ItemsSource = Devices;
            

        }


        readonly IList<IDisposable> cleanup = new List<IDisposable>();
        public ConnectionStatus Status { get; private set; } = ConnectionStatus.Disconnected;
        // public ObservableCollection<Group<GattCharacteristicViewModel>> GattCharacteristics { get; } = 
        //                                                   new ObservableCollection<Group<GattCharacteristicViewModel>>();
        public IGattCharacteristic CharacteristicRead { get; set; }
        public IGattCharacteristic CharacteristicWrite { get; set; }

        public int Rssi { get; private set; }
        private void cleanEverything()
        {
            /* this.cleanup.Add(this.device
                    .WhenNameUpdated()
                    .Subscribe(x => this.Name = this.device.Name)
            );*/

            this.cleanup.Add(this.device
                .WhenStatusChanged()
                .Subscribe(x => Device.BeginInvokeOnMainThread(() =>
                {
                    this.Status = x;

                    switch (x)
                    {
                        case ConnectionStatus.Disconnecting:
                        case ConnectionStatus.Connecting:
                            this.ConnectText = x.ToString();
                            break;

                        case ConnectionStatus.Disconnected:
                            this.ConnectText = "Connect";
                            Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).IsConnected = false;
                            Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).UpdateD();
                            //this.GattCharacteristics.Clear();
                            //this.GattDescriptors.Clear();
                            this.Rssi = 0;
                            break;

                        case ConnectionStatus.Connected:
                            this.ConnectText = "Disconnect";
                            Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).IsConnected = true;
                            Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).UpdateD();
                            //this.cleanup.Add(this.device
                            //    .WhenRssiUpdated()
                            //    .Subscribe(rssi => this.Rssi = rssi)
                            //);
                            break;
                    }
                }))
            );

            this.cleanup.Add(this.device
                .WhenMtuChanged()
                .Skip(1)
                .Subscribe(x => UserDialogs.Instance.Alert($"MTU Changed size to {x}"))
            );

            this.cleanup.Add(
            this.device.WhenAnyCharacteristicDiscovered().Subscribe(characteristic => {

                if (characteristic.Uuid.Equals(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e")))
                {
                    CharacteristicRead = characteristic;
                    // once you have your characteristic instance from the service discovery
                    // this will enable the subscriptions to notifications as well as actually hook to the event
                    var sub = CharacteristicRead.SubscribeToNotifications().Subscribe(result => 
                                      { result.Characteristic.SubscribeToNotifications().Subscribe(x => this.SetReadValue(x, true)); });

                }
                if (characteristic.Uuid.Equals(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e")))
                {
                    CharacteristicWrite= characteristic;
                }


            })
            );

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


                    if (!Value.Equals(tempval))
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
                    }

                }
                //this.Value = fromUtf8 ? Encoding.UTF8.GetString(result.Data, 0, result.Data.Length) : BitConverter.ToString(result.Data);

            });
        }

    }
}
