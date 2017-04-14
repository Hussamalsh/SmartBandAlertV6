using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Models
{
    public class DeviceListItemViewModel : INotifyPropertyChanged
    {
        public IDevice Device { get; private set; }

        public Guid Id => Device.Id;
        public bool IsConnected => Device.State == DeviceState.Connected;
        public int Rssi => Device.Rssi;
        public string Name => Device.Name;
        public string ButtonName => ((Device.State == DeviceState.Connected) ? "Avslut" : "Anslut");
        public DeviceListItemViewModel(IDevice device)
        {
            Device = device;
            OnPropertyChanged(nameof(ButtonName));
            OnPropertyChanged(nameof(IsConnected));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Update(IDevice newDevice = null)
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
