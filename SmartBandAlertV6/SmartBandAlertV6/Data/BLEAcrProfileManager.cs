using Acr.UserDialogs;
using Plugin.BluetoothLE;
using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartBandAlertV6.Data
{
    public class BLEAcrProfileManager
    {
        public BLEAcrProfile bleprofile;
        readonly IList<IDisposable> cleanup = new List<IDisposable>();
        public BLEAcrProfileManager()
        {
            bleprofile = new BLEAcrProfile();
            bleprofile.BleAdapter = CrossBleAdapter.Current;
        }


        public void intit()
        {
            bleprofile.connect = bleprofile.BleAdapter
                .WhenDeviceStatusChanged()
                .Subscribe(x =>
                {
                    var vm = bleprofile.Devices.FirstOrDefault(dev => dev.Uuid.Equals(x.Uuid));
                    if (vm != null)
                        vm.IsConnected = x.Status == ConnectionStatus.Connected;
                });

            bleprofile.BleAdapter.WhenStatusChanged().Subscribe(x => Device.BeginInvokeOnMainThread(() =>
            {
                bleprofile.IsSupported = x == AdapterStatus.PoweredOn;
                //this.Title = $"BLE Scanner ({x})";
            }
            ));

            //this.AppState.WhenBackgrounding().Subscribe(_ => this.scan?.Dispose());
        }

        public void cleanEverything(IDevice device)
        {
            /* this.cleanup.Add(this.device
                    .WhenNameUpdated()
                    .Subscribe(x => this.Name = this.device.Name)
            );*/
            bleprofile.device = device;
            cleanup.Add(bleprofile.device
                .WhenStatusChanged()
                .Subscribe(x => Device.BeginInvokeOnMainThread(() =>
                {
                    bleprofile.Status = x;

                    switch (x)
                    {
                        case ConnectionStatus.Disconnecting:
                        case ConnectionStatus.Connecting:
                            bleprofile.ConnectText = x.ToString();
                            break;

                        case ConnectionStatus.Disconnected:
                            bleprofile.ConnectText = "Connect";
                            bleprofile.Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).IsConnected = false;
                            bleprofile.Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).UpdateD();
                            //this.GattCharacteristics.Clear();
                            //this.GattDescriptors.Clear();
                            bleprofile.Rssi = 0;
                            break;

                        case ConnectionStatus.Connected:
                            bleprofile.ConnectText = "Disconnect";
                            bleprofile.Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).IsConnected = true;
                            bleprofile.Devices.FirstOrDefault(d => d.Device.Uuid == device.Uuid).UpdateD();
                            //this.cleanup.Add(this.device
                            //    .WhenRssiUpdated()
                            //    .Subscribe(rssi => this.Rssi = rssi)
                            //);
                            break;
                    }
                }))
            );

            this.cleanup.Add(bleprofile.device
                .WhenMtuChanged()
                .Skip(1)
                .Subscribe(x => UserDialogs.Instance.Alert($"MTU Changed size to {x}"))
            );

            /*this.cleanup.Add(
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
                    CharacteristicWrite = characteristic;
                }


            })
            );*/

        }




    }
}
