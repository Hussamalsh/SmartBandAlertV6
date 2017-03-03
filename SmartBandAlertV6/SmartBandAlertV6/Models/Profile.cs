using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Models
{
    public class Profile
    {
        public bool HaveSmartBand
        {
            get;
            set;
        }

        public String BlegUID
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string FBusername
        {
            get;
            set;
        }

        public string FBimage
        {
            get;
            set;
        }

        public String FBid
        {
            get;
            set;
        }

        public string Firstname
        {
            get;
            set;
        }

        public bool NotifyNews
        {
            get;
            set;
        }

        public bool NotifyFriends
        {
            get;
            set;
        }

    }
}
