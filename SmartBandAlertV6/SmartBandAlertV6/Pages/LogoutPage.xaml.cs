using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV6.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();

            OnLoginButtonClicked();


        }

        bool authenticated = false;

        async void OnLoginButtonClicked()
        {
             try
             {

                 if (App.Authenticator != null)
                 {
                     authenticated =  App.Authenticator.LogoutAsync();
                 }

                 if (authenticated)
                 {
                    //Navigation.InsertPageBefore(new MainPageCS(), this);
                    //await Navigation.PopAsync();
                    //Application.Current.MainPage = new MainPageCS();

                    //await App.Current.MainPage.Navigation.PushModalAsync(new FBLoginPage(providername));
                    App.IsLoggedIn = false;
                    ((App)App.Current).PresentMainPage();

                }
            }
             catch (InvalidOperationException ex)
             {
                 if (ex.Message.Contains("Authentication was cancelled"))
                 {
                     messageLabel.Text = "Authentication cancelled by the user";
                 }
             }
             catch (Exception)
             {
                 messageLabel.Text = "Authentication failed";
             }

        }



    }
}
