using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public interface IBLEProfileManager : INotifyPropertyChanged
    {
        /* void StartUpdates(ICharacteristic characteristicT);
         void ReadValueAsync(ICharacteristic characteristicT);

         void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs);*/
        void PostData();

    }
}
