using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public class BLEProfileManager
    {
        private CancellationTokenSource _cancellationTokenSource;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public BLEProfile bleprofile { get; set; } = new BLEProfile();

        public BLEProfileManager()
        {

        }



       public void init()
        {
            bleprofile.ble = CrossBluetoothLE.Current;
            bleprofile.Adapter = CrossBluetoothLE.Current.Adapter;

            bleprofile.Adapter.DeviceDisconnected += OnDeviceDisconnected;
            bleprofile.Adapter.DeviceConnectionLost += OnDeviceConnectionLost;
            bleprofile.Adapter.DeviceDiscovered += OnDeviceDiscovered;
            bleprofile.Adapter.StartScanningForDevicesAsync();
        }


        public async void ScanForDevices()
        {
            if (bleprofile.Devices.Count != 0)
                bleprofile.Devices.Clear();

            foreach (var connectedDevice in bleprofile.Adapter.ConnectedDevices)
            {
                //update rssi for already connected evices (so tha 0 is not shown in the list)
                try
                {
                    connectedDevice.UpdateRssiAsync();
                }
                catch (Exception ex)
                {
                    //Mvx.Trace(ex.Message);
                    //_userDialogs.ShowError($"Failed to update RSSI for {connectedDevice.Name}");
                }

                AddOrUpdateDevice(connectedDevice);
            }

            _cancellationTokenSource = new CancellationTokenSource();
            // RaisePropertyChanged(() => StopScanCommand);

            bleprofile.Adapter.StartScanningForDevicesAsync(null, null, false, _cancellationTokenSource.Token);
            //RaisePropertyChanged(() => IsRefreshing);
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            AddOrUpdateDevice(args.Device);
        }


        private void AddOrUpdateDevice(IDevice device)
        {


            var vm = bleprofile.Devices.FirstOrDefault(d => d.Device.Id == device.Id);
            if (vm != null)
            {
                vm.Update();
            }
            else
            {
                if (device.Name != null)
                    bleprofile.Devices.Add(new DeviceListItemViewModel(device));
            }

            //this.Devicesl.ItemsSource = this.Devices;

           /* if (bleprofile.Devices.Count != 0)
            {
                string s = bleprofile.Devices.FirstOrDefault().Device.State.ToString();
                if (s.Equals("Disconnected"))
                {
                    bleprofile.Adapter.ConnectToDeviceAsync(bleprofile.Devices.FirstOrDefault().Device);
                    checkservice();
                }
            }*/

        }

        /*
        public async void checkservice()
        {

            if (bleprofile.Devices.Count != 0)
            {
                //string s = bleprofile.Devices.FirstOrDefault().Device.State.ToString();
                /* if (s.Equals("Disconnected"))
                 {
                     bleprofile.Adapter.ConnectToDeviceAsync(bleprofile.Devices.FirstOrDefault().Device);
                     */

                /*foreach (DeviceListItemViewModel item in bleprofile.Devices)
                    if (item.Name != null)
                    {
                        if (item.Name.Equals("SmartBandAlert"))
                        {
                            IDevice d = item.Device;
                            bleprofile.Adapter.ConnectToDeviceAsync(d);
                            bleprofile.Services = await d.GetServicesAsync();
                            //Services = d.GetServicesAsync();
                            while (bleprofile.Services.Count == 0)
                            {
                                bleprofile.Services = await d.GetServicesAsync();
                            }

                            checkchar();

                        }
                    }*/

                //}

               /* IDevice d = bleprofile.Devices.FirstOrDefault().Device;
                bleprofile.Adapter.ConnectToDeviceAsync(d);

                bleprofile.Services = await d.GetServicesAsync();
                while (bleprofile.Services.Count == 0)
                {
                    bleprofile.Services = await d.GetServicesAsync();
                }

                checkchar();
            }




        }*/

       /* async void checkchar()
        {
            foreach (IService item in bleprofile.Services)
                if (item.Name != null)
                {
                    if (item.Name.Equals("Unknown Service"))
                    {
                        bleprofile.Characteristics = new List<ICharacteristic>(await item.GetCharacteristicsAsync());

                    }
                }

            choosecharWrite();
        }*/


        async void choosechar()
        {

            foreach (ICharacteristic item in bleprofile.Characteristics)
                if (item.Name != null)
                {
                    if (item.Uuid.Equals("6e400003-b5a3-f393-e0a9-e50e24dcca9e"))
                    {
                        bleprofile.CharacteristicT = item;
                        StartUpdates();
                        ReadValueAsync();
                        // WriteValueAsync();
                    }
                }
            choosecharWrite();
        }

        async void choosecharWrite()
        {

            foreach (ICharacteristic item in bleprofile.Characteristics)
                if (item.Name != null)
                {
                    if (item.Uuid.Equals("6e400002-b5a3-f393-e0a9-e50e24dcca9e"))
                    {
                        bleprofile.CharacteristicT = item;
                        //StartUpdates();
                        //ReadValueAsync();
                        WriteValueAsync();
                    }
                }

        }


        private async Task WriteValueAsync()
        {
            
            bleprofile.CharacteristicT = await bleprofile.Services.GetCharacteristicAsync(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e"));

            try
            {

                //var data = GetBytes("11");
                byte[] toBytes = Encoding.UTF8.GetBytes("11");


                // RxChar.setValue(data.getBytes(Charset.forName("UTF-8")));

                bleprofile.CharacteristicT.WriteAsync(toBytes);

                //RaisePropertyChanged(() => CharacteristicValue);

                //OnPropertyChanged((nameof(bleprofile.CharacteristicValue)));


                // DisplayAlert("value", CharacteristicValue, "ok");
                //Messages.Insert(0, $"Wrote value {CharacteristicValue}");
            }
            catch (Exception ex)
            {

            }

        }


        public async void ReadValueAsync()
        {
            if (bleprofile.CharacteristicT == null)
                return;

            try
            {
                //_userDialogs.ShowLoading("Reading characteristic value...");

                bleprofile.CharacteristicT.ReadAsync();

                // RaisePropertyChanged(() => CharacteristicValue);
                OnPropertyChanged(nameof(bleprofile.CharacteristicValue));

                // Messages.Insert(0, $"Read value {CharacteristicValue}");
            }
            catch (Exception ex)
            {
                //_userDialogs.HideLoading();
                // _userDialogs.ShowError(ex.Message);

                //Messages.Insert(0, $"Error {ex.Message}");

            }
            finally
            {
                //_userDialogs.HideLoading();
            }

        }

        public async void StartUpdates()
        {
            try
            {
                bleprofile._updatesStarted = true;

                bleprofile.CharacteristicT.ValueUpdated -= CharacteristicOnValueUpdated;
                bleprofile.CharacteristicT.ValueUpdated += CharacteristicOnValueUpdated;
                bleprofile.CharacteristicT.StartUpdatesAsync();


                //Messages.Insert(0, $"Start updates");

                //RaisePropertyChanged(() => UpdateButtonText);
                OnPropertyChanged(nameof(bleprofile.UpdateButtonText));
            }
            catch (Exception ex)
            {
                //_userDialogs.ShowError(ex.Message);
            }
        }

        public string batteryLevel;
        private void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            getcharacteristicRXAsync();
            // Messages.Insert(0, $"Updated value: {CharacteristicValue}");
            //RaisePropertyChanged(() => CharacteristicValue);
            OnPropertyChanged(nameof(bleprofile.CharacteristicValue));

            
            if (!String.IsNullOrEmpty(bleprofile.CharacteristicValue))
                batteryLevel = System.Text.Encoding.UTF8.GetString(bleprofile.CharacteristicT.Value, 0, bleprofile.CharacteristicT.Value.Length);
        }

        private static byte[] GetBytes(string text)
        {
            return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
        }


        public string getBatterylevelAsync()
        {

           /* StartUpdates();
            ReadValueAsync();*/

           
            WriteValueAsync();


            while (String.IsNullOrEmpty(batteryLevel))
            {

            }

            return batteryLevel;
        }

        public async Task waitAsync()
        {
            await Task.Delay(2000);
        }


        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            bleprofile.Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            //DisplayAlert("disconnected", "Try again ur disconnected", "Ok");
        }

        private void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            bleprofile.Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();

            //DisplayAlert("connection lost", "Try again ur disconnected", "Ok");
        }

        public async void getUnknownServiceAsync()
        {
            bleprofile.Services = await bleprofile.Adapter.ConnectedDevices.FirstOrDefault().GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
            getcharacteristicRXAsync();
        }
        public async void getcharacteristicRXAsync()
        {
            bleprofile.CharacteristicT = await bleprofile.Services.GetCharacteristicAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
            StartUpdates();
        }


    }
}
