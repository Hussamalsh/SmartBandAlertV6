using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public class ProfileManager : IProfileManager
    {
        public ProfileManager()
        {
        }

        public void SaveProfile(Profile profile)
        {
            App.Current.Properties[nameof(Profile.Username)] = profile.Username;

            App.Current.Properties[nameof(Profile.FBusername)] = profile.FBusername;
            App.Current.Properties[nameof(Profile.FBimage)] = profile.FBimage;
            App.Current.Properties[nameof(Profile.FBid)] = profile.FBid;
            App.Current.Properties[nameof(Profile.HaveSmartBand)] = profile.HaveSmartBand;
            App.Current.Properties[nameof(Profile.BlegUID)] = profile.BlegUID;
            App.Current.Properties[nameof(Profile.FBemail)] = profile.FBemail;

            App.Current.Properties[nameof(Profile.Firstname)] = profile.Firstname;
            App.Current.Properties[nameof(Profile.NotifyNews)] = profile.NotifyNews;
            App.Current.Properties[nameof(Profile.NotifyFriends)] = profile.NotifyFriends;
            App.Current.SavePropertiesAsync();
        }

        public Profile LoadProfile()
        {
            var profile = new Profile
            {
                Username = App.Current.Properties.ContainsKey(nameof(Profile.Username))
                    ? App.Current.Properties[nameof(Profile.Username)] as string
                    : string.Empty,

                FBusername = App.Current.Properties.ContainsKey(nameof(Profile.FBusername))
                    ? App.Current.Properties[nameof(Profile.FBusername)] as string
                    : string.Empty,

                FBimage = App.Current.Properties.ContainsKey(nameof(Profile.FBimage))
                    ? App.Current.Properties[nameof(Profile.FBimage)] as string
                    : string.Empty,

                FBid = App.Current.Properties.ContainsKey(nameof(Profile.FBid))
                    ? App.Current.Properties[nameof(Profile.FBid)] as string
                    : string.Empty,


                HaveSmartBand = App.Current.Properties.ContainsKey(nameof(Profile.HaveSmartBand))
                    ? (bool)App.Current.Properties[nameof(Profile.HaveSmartBand)]
                    : false,
                BlegUID = App.Current.Properties.ContainsKey(nameof(Profile.BlegUID))
                    ? App.Current.Properties[nameof(Profile.BlegUID)] as string
                    : string.Empty,

                FBemail = App.Current.Properties.ContainsKey(nameof(Profile.FBemail))
                    ? App.Current.Properties[nameof(Profile.FBemail)] as string
                    : string.Empty,



                Firstname = App.Current.Properties.ContainsKey(nameof(Profile.Firstname))
                    ? App.Current.Properties[nameof(Profile.Firstname)] as string
                    : string.Empty,
                NotifyNews = App.Current.Properties.ContainsKey(nameof(Profile.NotifyNews))
                    ? (bool)App.Current.Properties[nameof(Profile.NotifyNews)]
                    : false,
                NotifyFriends = App.Current.Properties.ContainsKey(nameof(Profile.NotifyFriends))
                    ? (bool)App.Current.Properties[nameof(Profile.NotifyFriends)]
                    : false
            };
            return profile;
        }

        public void PurgeProfile()
        {
            App.Current.Properties[nameof(Profile.Username)] = string.Empty;
            App.Current.Properties[nameof(Profile.Firstname)] = string.Empty;
            App.Current.Properties[nameof(Profile.NotifyNews)] = true;
            App.Current.Properties[nameof(Profile.NotifyFriends)] = true;
        }
    }
}
