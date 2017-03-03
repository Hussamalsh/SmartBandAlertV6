using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Models
{
    public class FriendsList
    {
        public String FriendFBID { get; set; }
        public String UserName { get; set; }
        public FriendsList(string n)
        {
            UserName = n;
        }

        public String ImgLink { get; set; }

    }
}
