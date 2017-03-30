using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using SmartBandAlertV6.Pages;
using SmartBandAlertV6.iOS.Renderers;
using System.Threading.Tasks;
using Xamarin.Auth;
using System.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;

[assembly: ExportRenderer(typeof(FBLoginPage), typeof(FBLoginPageRenderer))]

namespace SmartBandAlertV6.iOS.Renderers
{
    public class FBLoginPageRenderer : PageRenderer
    {
        private readonly TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            LoginToFacebook(false);
        }

        void LoginToFacebook(bool allowCancel)
        {
            var loginPage = Element as FBLoginPage;
            string providername = loginPage.ProviderName;


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
                        //saveset(obj["id"], obj["name"]);

                        // Get Shared User Defaults
                        var plist = NSUserDefaults.StandardUserDefaults;
                        // Save value
                        plist.SetString(obj["id"], "PrefId");
                        plist.SetString(obj["name"], "PrefName");
                        // Sync changes to database
                        plist.Synchronize();

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
                        // saveset(obj["id"], obj["name"]);
                        
                        // Get Shared User Defaults
                        var plist = NSUserDefaults.StandardUserDefaults;
                        // Save value
                        plist.SetString(obj["id"], "PrefId");
                        plist.SetString(obj["name"], "PrefName");
                        // Sync changes to database
                        plist.Synchronize();


                    }

                    // On Android: store the account
                    //AccountStore.Create(Context).Save(eargs.Account, "Facebook");
                    //AccountStore.Create().Save(eargs.Account, "Facebook");  //Account object is securely saved on the iOS platform

                    //Save as a new user to the database
                    await App.UserManager.SaveTaskAsync
                            (new Models.User { FBID = App.FacebookId, UserName = App.FacebookName, Email = App.EmailAddress, ImgLink = App.ProfilePic }, true);


                    //retreive gcm id
                   // var prefs = Android.App.Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
                    //string id = prefs.GetString("regId", null);

                    //RegisterAsync(id, new string[] { App.FacebookId + "T" }, App.FacebookId);

                    await App.Current.MainPage.Navigation.PopModalAsync();
                    App.IsLoggedIn = true;
                    ((App)App.Current).SaveProfile();
                    ((App)App.Current).PresentMainPage();
                }
            };


            UIViewController vc = (UIViewController)auth.GetUI();

            ViewController.AddChildViewController(vc);
            ViewController.View.Add(vc.View);

            vc.ChildViewControllers[0].NavigationItem.LeftBarButtonItem = new UIBarButtonItem(
                UIBarButtonSystemItem.Cancel, async (o, eargs) => await App.Current.MainPage.Navigation.PopModalAsync()
            );
        }



        private class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }

            public string Friendid { get; set; }
        }

        //POST_URL = backendEndpoint + "/api/register";
        private string POST_URL = "http://sbat1.azurewebsites.net/api/register";
        HttpClient client;

        public async Task RegisterAsync(string handle, IEnumerable<string> tags, string fid)
        {

            client = new HttpClient();
            client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.MaxResponseContentBufferSize = 256000;

            var regId = await RetrieveRegistrationIdOrRequestNewOneAsync(handle);

            var deviceRegistration = new DeviceRegistration
            {
                Platform = "gcm",
                Handle = handle,
                Tags = tags.ToArray<string>(),
                Friendid = fid
            };


            var statusCode = await UpdateRegistrationAsync(regId, deviceRegistration);

            if (statusCode == HttpStatusCode.OK)
                return;


            if (statusCode == HttpStatusCode.Gone)
            {
                // regId is expired, deleting from local storage & recreating
                // var settings = ApplicationData.Current.LocalSettings.Values;
                // settings.Remove("__NHRegistrationId");
                regId = await RetrieveRegistrationIdOrRequestNewOneAsync(handle);
                statusCode = await UpdateRegistrationAsync(regId, deviceRegistration);
            }

            if (statusCode != HttpStatusCode.Accepted)
            {
                // log or throw
                throw new System.Net.WebException(statusCode.ToString());
            }
        }


        //////here we add persons....
        private async Task<HttpStatusCode> UpdateRegistrationAsync(string regId, DeviceRegistration deviceRegistration)
        {




            using (var httpClient = new HttpClient())
            {
                // var settings = ApplicationData.Current.LocalSettings.Values;
                // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", (string)settings["AuthenticationToken"]);

                var putUri = POST_URL + "/" + regId;

                //
                var obj = JsonConvert.SerializeObject(deviceRegistration, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var request = new HttpRequestMessage(HttpMethod.Put, putUri);
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");
                var response = client.SendAsync(request).Result;
                return response.StatusCode;
            }
        }

        private async Task<string> RetrieveRegistrationIdOrRequestNewOneAsync(string handle)
        {
            //var settings = ApplicationData.Current.LocalSettings.Values;
            //if (!settings.ContainsKey("__NHRegistrationId"))
            //{
            string regId;
            using (var httpClient = new HttpClient())
            {
                // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", (string)settings["AuthenticationToken"]);


                var obj = JsonConvert.SerializeObject("", new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var request = new HttpRequestMessage(HttpMethod.Post, POST_URL + "?handle=" + handle);
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).Result;

                // var response = await httpClient.PostAsync(POST_URL, new StringContent("?handle=" + handle));
                if (response.IsSuccessStatusCode)
                {
                    regId = await response.Content.ReadAsStringAsync();
                    regId = regId.Substring(1, regId.Length - 2);
                    //settings.Add("__NHRegistrationId", regId);
                }
                else
                {
                    throw new System.Net.WebException(response.StatusCode.ToString());
                }
            }
            //}
            return regId;

        }


    }
}