using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Models
{
    public class BLEAcrProfile
    {

        public IAdapter BleAdapter;
        public ObservableCollection<ScanResultViewModel> Devices { get; set; } = new ObservableCollection<ScanResultViewModel>();
        public IDisposable connect;
       
        public IGattCharacteristic CharacteristicRead { get; set; }
        public IGattCharacteristic CharacteristicWrite { get; set; }
        public IDevice device;
        public ConnectionStatus Status { get; set; } = ConnectionStatus.Disconnected;
        public int Rssi { get; set; }
        public string ConnectText { get; set; } = "Connect";
        public bool IsSupported { get; set; }

        public BLEAcrProfile()
        {
        }

    }
}
