using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            Title = "Login";
            InitializeComponent();
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {

            Button btncontrol = (Button)sender;

            await FacebookLogin(btncontrol.Text);
        }

        protected async Task FacebookLogin(string providername)
        {

            //await LoadingLoginNoCancel("Accessing Facebook ...");
            await App.Current.MainPage.Navigation.PushModalAsync(new FBLoginPage(providername));
        }

        async Task LoadingLoginNoCancel(string msg, int time = 500)
        {
            //using (UserDialogs.Instance.Loading(msg, null, null, true, MaskType.Black))
            // await Task.Delay(TimeSpan.FromMilliseconds(time));
        }

    }
}
