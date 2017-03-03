using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public interface IProfileManager
    {
        void SaveProfile(Profile profile);

        Profile LoadProfile();
    }
}
