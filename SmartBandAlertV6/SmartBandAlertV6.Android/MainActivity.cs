using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Xamarin.Auth;
using System.Linq;
using Android.Media;
using Gcm.Client;
using SmartBandAlertV6.Droid.Services;
using Xamarin.Forms;
using SmartBandAlertV6.Messages;

namespace SmartBandAlertV6.Droid
{
    [Activity(Label = "SmartBandAlertV6", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MediaPlayer _player;

        Intent startServiceIntent;
        Intent stopServiceIntent;
        bool isStarted = false;

        public static MainActivity CurrentActivity { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Set the current instance of MainActivity.
            CurrentActivity = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            if (!App.NotificationOn)
                _player = MediaPlayer.Create(this, Resource.Raw.siren2);

            base.OnCreate(savedInstanceState);

            //SetContentView(Resource.Layout.Main);
            OnNewIntent(this.Intent);



            if (savedInstanceState != null)
            {
                isStarted = savedInstanceState.GetBoolean(Constants.SERVICE_STARTED_KEY, false);
            }

            //wire up
            WireUpLongRunningTask();
           /*startServiceIntent = new Intent(this, typeof(TimestampService));
            startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);


            stopServiceIntent = new Intent(this, typeof(TimestampService));
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);*/


            // On Android:
            var accounts = AccountStore.Create(this).FindAccountsForService("Facebook");
            var account = accounts.FirstOrDefault();
            if (account != null)
                App.IsLoggedIn = true;

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            Xamarin.FormsMaps.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);


            LoadApplication(new App());

            //StartService(startServiceIntent);

            try
            {
                // Check to ensure everything's setup right
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);

                // Register for push notifications
                System.Diagnostics.Debug.WriteLine("Registering...");
                GcmClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);
            }
            catch (Java.Net.MalformedURLException)
            {
                CreateAndShowDialog("There was an error creating the client. Verify the URL.", "Error");
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e.Message, "Error");
            }
        }

        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", message => {

                startServiceIntent = new Intent(this, typeof(TimestampService));
                startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);
                StartService(startServiceIntent);
                

            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message => {
                stopServiceIntent = new Intent(this, typeof(TimestampService));
                stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
                StopService(stopServiceIntent);
            });



            

           
        }

        private void CreateAndShowDialog(String message, String title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }


        protected override void OnNewIntent(Intent intent)
        {
            if (intent == null)
            {
                return;
            }

            var bundle = intent.Extras;
            if (bundle != null)
            {
                if (bundle.ContainsKey(Constants.SERVICE_STARTED_KEY))
                {
                    isStarted = true;
                }
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(Constants.SERVICE_STARTED_KEY, isStarted);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

    }
}

