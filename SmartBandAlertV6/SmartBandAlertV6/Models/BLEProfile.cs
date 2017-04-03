using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Models
{
    public class BLEProfile : INotifyPropertyChanged
    {
        public IAdapter Adapter { get; set; }
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();
        public IBluetoothLE ble { get; set; }
        public IService Services { get; set; }
        private IList<ICharacteristic> _characteristics;

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<ICharacteristic> Characteristics
        {
            get { return _characteristics; }
             set
            {
                _characteristics = value;
                OnPropertyChanged((nameof(_characteristics)));

            }

        }


        public ICharacteristic CharacteristicT { get;  set; }


        public bool IsScanning => Adapter.IsScanning;

        public bool _updatesStarted { get; set; }
        public string UpdateButtonText => _updatesStarted ? "Stop updates" : "Start updates";

        public string Permissions
        {
            get
            {
                if (CharacteristicT == null)
                    return string.Empty;

                return (CharacteristicT.CanRead ? "Read " : "") +
                       (CharacteristicT.CanWrite ? "Write " : "") +
                       (CharacteristicT.CanUpdate ? "Update" : "");
            }
        }
        public string CharacteristicValue => CharacteristicT?.Value.ToHexString().Replace("-", " ");


        public List<DeviceListItemViewModel> SystemDevices { get;  set; }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
