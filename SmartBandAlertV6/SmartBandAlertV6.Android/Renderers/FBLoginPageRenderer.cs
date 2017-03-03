using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using SmartBandAlertV6.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using SmartBandAlertV6.Pages;
using System.Json;

[assembly: ExportRenderer(typeof(FBLoginPage), typeof(FBLoginPageRenderer))]

namespace SmartBandAlertV6.Droid.Renderers
{
    public class FBLoginPageRenderer : PageRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            LoginToFacebook(false);
        }

        void LoginToFacebook(bool allowCancel)
        {
            var loginPage = Element as FBLoginPage;
            string providername = loginPage.ProviderName;
            var activity = this.Context as Activity;


            OAuth2Authenticator auth = null;
            switch (providername)
            {
                case "Google":
                    {
                        auth = new OAuth2Authenticator(
                                    // For Google login, for configure refer https://code.msdn.microsoft.com/Register-Identity-Provider-41955544
                                    App.ClientId,
                                   App.ClientSecret,
                                    // Below values do not need changing
                                    "https://www.googleapis.com/auth/userinfo.email",
                                    new Uri(App.url2),
                                    new Uri(App.url3),// Set this property to the location the user will be redirected too after successfully authenticating
                                    new Uri(App.url4)
                                    );

                        break;
                    }
                case "FaceBook":
                    {
                        auth = new OAuth2Authenticator(
                        clientId: App.AppId,
                        scope: App.ExtendedPermissions,
                        authorizeUrl: new Uri(App.AuthorizeUrl),
                        redirectUrl: new Uri(App.RedirectUrl));
                        break;
                    }
            }

            auth.AllowCancel = allowCancel;

            // If authorization succeeds or is canceled, .Completed will be fired.
            auth.Completed += async (s, eargs) =>
            {
                if (!eargs.IsAuthenticated)
                {
                    return;
                }
                else
                {

                    //var token = eargs.Account.Properties["access_token"];

                    if (providername.Equals("FaceBook"))
                    {

                        // Now that we're logged in, make a OAuth2 request to get the user's info.
                        var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=id,name,picture,email"), null, eargs.Account);
                        var result = await request.GetResponseAsync();

                        string resultText = result.GetResponseText();
                        var obj = JsonValue.Parse(resultText);
                        // Console.WriteLine(token + " -=- " + resultText);

                        App.FacebookId = obj["id"];
                        App.FacebookName = obj["name"];
                        App.EmailAddress = obj["email"];
                        App.ProfilePic = obj["picture"]["data"]["url"];
                        //
                        saveset(obj["id"], obj["name"]);

                    }
                    else
                    {
                        var request = new OAuth2Request("GET", new Uri("https://www.googleapis.com/plus/v1/people/me/openIdConnect"), null, eargs.Account);
                        var result = await request.GetResponseAsync();

                        string resultText = result.GetResponseText();
                        var obj = JsonValue.Parse(resultText);

                        /*string username = (string)obj["name"];
                        string email = (string)obj["email"];*/



                        App.FacebookId = obj["sub"];
                        App.FacebookName = obj["name"];
                        App.EmailAddress = obj["email"];
                        App.ProfilePic = obj["picture"];
                        //
                        saveset(obj["id"], obj["name"]);

                    }

                    // On Android: store the account
                    AccountStore.Create(Context).Save(eargs.Account, "Facebook");
                    //Save as a new user to the database
                    await App.UserManager.SaveTaskAsync
                                  (new Models.User { FBID = App.FacebookId, UserName = App.FacebookName, Email = App.EmailAddress, ImgLink = App.ProfilePic }, true);

                    await App.Current.MainPage.Navigation.PopModalAsync();
                    App.IsLoggedIn = true;
                   ((App)App.Current).SaveProfile();
                    ((App)App.Current).PresentMainPage();
                }
            };


            var intent = auth.GetUI(activity);
            activity.StartActivity(intent);
        }

        protected void saveset(string id, string name)
        {

            //store
            var prefs = Android.App.Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("PrefId", id);
            prefEditor.PutString("PrefName", name);
            prefEditor.Commit();

        }


    }
}