using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync();

        bool LogoutAsync();

    }
}
