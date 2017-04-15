using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Models
{
    public class ScanResultViewModel : INotifyPropertyChanged
    {
        //IDisposable nameOb;
        public IDevice Device { get; private set; }
        public string Name { get; private set; }
        public bool IsConnected { get; set; }
        public Guid Uuid { get; private set; }
        public int Rssi { get; private set; }
        public bool IsConnectable { get; private set; }
        public int ServiceCount { get; private set; }
        public string ManufacturerData { get; private set; }
        public string LocalName { get; private set; }
        public int TxPower { get; private set; }


        public string ButtonName => ((IsConnected) ? "Avsluta" : "Ansluta");

        //public ConnectionStatus Status { get; private set; } = ConnectionStatus.Disconnected;

        public bool TrySet(IScanResult result)
        {
            var response = false;

            if (this.Uuid == Guid.Empty)
            {
                this.Device = result.Device;
                this.Uuid = this.Device.Uuid;
                //this.nameOb = result
                //    .Device
                //    .WhenNameUpdated()
                //    .Subscribe(x => this.Name = x);

                response = true;
            }

            try
            {
                if (this.Uuid == result.Device.Uuid)
                {
                    response = true;

                    this.Name = result.Device.Name;
                    this.Rssi = result.Rssi;

                    var ad = result.AdvertisementData;
                    this.ServiceCount = ad.ServiceUuids?.Length ?? 0;
                    this.IsConnectable = ad.IsConnectable;
                    this.LocalName = ad.LocalName;
                    this.TxPower = ad.TxPower;
                    this.ManufacturerData = ad.ManufacturerData == null
                        ? null
                        : BitConverter.ToString(ad.ManufacturerData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            OnPropertyChanged(nameof(IsConnected));
            OnPropertyChanged(nameof(ButtonName));
           

            return response;
        }




        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateD(IDevice newDevice = null)
        {
            if (newDevice != null)
            {
                Device = newDevice;
            }
            OnPropertyChanged(nameof(IsConnected));
            OnPropertyChanged(nameof(Rssi));
            OnPropertyChanged(nameof(ButtonName));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return Device.Name;
        }

    }
}
