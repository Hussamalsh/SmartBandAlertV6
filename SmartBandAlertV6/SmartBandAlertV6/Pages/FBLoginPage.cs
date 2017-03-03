using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartBandAlertV6.Pages
{
    public class FBLoginPage : ContentPage
    {
        public string ProviderName { get; set; }
        public FBLoginPage(string _providername)
        {
            ProviderName = _providername;
        }
    }
}
