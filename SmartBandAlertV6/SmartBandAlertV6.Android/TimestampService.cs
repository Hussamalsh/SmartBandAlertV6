using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Runtime;
using System.Collections.ObjectModel;

using Android.Util;
using SmartBandAlertV6.Models;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Plugin.Geolocator;

using Plugin.BluetoothLE;
using Xamarin.Forms;
using System.Text;

namespace SmartBandAlertV6.Droid
{
    /// <summary>
	/// This is a sample started service. When the service is started, it will log a string that details how long 
	/// the service has been running (using Android.Util.Log). This service displays a notification in the notification
	/// tray while the service is active.
	/// </summary>
	[Service]
    public class TimestampService : Service
    {


        static readonly string TAG = typeof(TimestampService).FullName;

        //UtcTimestamper timestamper;
        bool isStarted;
        Handler handler;
        Action runnable;


        CancellationTokenSource _cts;


        //added by me 
        //public IAdapter BleAdapter;
        public BLEAcrProfile bleProfile;

        public Victim viktem;




        public override void OnCreate()
        {
            base.OnCreate();
            //Log.Info(TAG, "OnCreate: the service is initializing.");

            //timestamper = new UtcTimestamper();
            handler = new Handler();

            // This Action is only for demonstration purposes.
            runnable = new Action(() =>
            {
                string msg = "GetFormattedTimestamp";//timestamper.GetFormattedTimestamp();
                                                     //Log.Debug(TAG, msg);
                Intent i = new Intent(Constants.NOTIFICATION_BROADCAST_ACTION);
                i.PutExtra(Constants.BROADCAST_MESSAGE_KEY, msg);
                Android.Support.V4.Content.LocalBroadcastManager.GetInstance(this).SendBroadcast(i);
                handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_LOG_MESSAGES);
            });
        }
        IDisposable subNoTIFYB;
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    //Log.Info(TAG, "OnStartCommand: The service is already running.");
                }
                else
                {
                    viktem = new Victim();
                    //to add

                    //BleAdapter = App.BLEAcrProfileManager.bleprofile.BleAdapter;
                    bleProfile = App.BLEAcrProfileManager.bleprofile;

                    /*subNoTIFYB = bleProfile.CharacteristicRead.SubscribeToNotifications().Subscribe(result =>
                    {
                        /* result.Characteristic.SubscribeToNotifications().Subscribe
                         (x => this.SetReadValueB(x, true));*/

                     /*   if (result.Data != null)
                             PostData();

                    });*/


                    //Log.Info(TAG, "OnStartCommand: The service is starting.");
                    RegisterForegroundService();
                    handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_LOG_MESSAGES);
                    isStarted = true;
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                //Log.Info(TAG, "OnStartCommand: The service is stopping.");
                //timestamper = null;
                StopForeground(true);
                StopSelf();
                isStarted = false;

            }
            else if (intent.Action.Equals(Constants.ACTION_RESTART_TIMER))
            {
                // Log.Info(TAG, "OnStartCommand: Restarting the timer.");
                //timestamper.Restart();

            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// ///Bleeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        /// </summary>
        /// <param name="result"></param>
        /// <param name="fromUtf8"></param>
        //public string Value { get; private set; }
        void SetReadValueB(CharacteristicResult result, bool fromUtf8)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                // this.IsValueAvailable = true;
                //this.LastValue = DateTime.Now;

                if (result.Data == null)
                    PostData();
                /* this.Value = "EMPTY";
                 else
                 {
                     this.Value = fromUtf8 ? Encoding.UTF8.GetString(result.Data, 0, result.Data.Length) : BitConverter.ToString(result.Data);
                     if (!String.IsNullOrEmpty(this.Value))
                         PostData();
                 }*/
            });




        }

        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }


        public override void OnDestroy()
        {
            // We need to shut things down.
            // Log.Debug(TAG, /*GetFormattedTimestamp() ??*/ "The TimeStamper has been disposed.");
            // Log.Info(TAG, "OnDestroy: The started service is shutting down.");

            // Stop the handler.
            handler.RemoveCallbacks(runnable);

            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(Constants.SERVICE_RUNNING_NOTIFICATION_ID);

            //timestamper = null;
            isStarted = false;
            base.OnDestroy();
        }

        /// <summary>
        /// This method will return a formatted timestamp to the client.
        /// </summary>
        /// <returns>A string that details what time the service started and how long it has been running.</returns>
        /*string GetFormattedTimestamp()
        {

            return timestamper?.GetFormattedTimestamp();
        }*/

        void RegisterForegroundService()
        {
            var notification = new Notification.Builder(this)
                .SetContentTitle("SmartBand Alert")
                .SetContentText("Danger mode Test")
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .AddAction(BuildRestartTimerAction())
                .AddAction(BuildStopServiceAction())
                .Build();


            // Enlist this instance of the service as a foreground service
            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        /// <summary>
        /// Builds a PendingIntent that will display the main activity of the app. This is used when the 
        /// user taps on the notification; it will take them to the main activity of the app.
        /// </summary>
        /// <returns>The content intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        /// <summary>
        /// Builds a Notification.Action that will instruct the service to restart the timer.
        /// </summary>
        /// <returns>The restart timer action.</returns>
        Notification.Action BuildRestartTimerAction()
        {
            var restartTimerIntent = new Intent(this, GetType());
            restartTimerIntent.SetAction(Constants.ACTION_RESTART_TIMER);
            var restartTimerPendingIntent = PendingIntent.GetService(this, 0, restartTimerIntent, 0);

            var builder = new Notification.Action.Builder(Resource.Drawable.icon,
                                              "restart_timer",
                                              restartTimerPendingIntent);

            return builder.Build();
        }

        /// <summary>
        /// Builds the Notification.Action that will allow the user to stop the service via the
        /// notification in the status bar
        /// </summary>
        /// <returns>The stop service action.</returns>
        Notification.Action BuildStopServiceAction()
        {
            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            var builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause,
                                                         "stop_service",
                                                          stopServicePendingIntent);
            return builder.Build();

        }


        public async void getLocation()
        {
            var locator = CrossGeolocator.Current;

            locator.DesiredAccuracy = 50;

            var position1 = locator.GetPositionAsync(timeoutMilliseconds: 10000);
            Plugin.Geolocator.Abstractions.Position position = position1.Result;
            if (position == null)
            {
                return;
            }

            var prefs = Android.App.Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);


            viktem.UserName = prefs.GetString("PrefName", null);
            viktem.FBID = prefs.GetString("PrefId", null);

            viktem.StartDate = DateTime.Parse(position.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            viktem.Latitude = "" + position.Latitude;
            viktem.Longitude = "" + position.Longitude;
            // Victim.Adress = "" + possibleAddresses.FirstOrDefault();

        }



        public async void PostData()
        {
            /* var deviceInformation = new DeviceInformation();
             deviceInformation.BatteryStatus = 1;
             deviceInformation.LocationLatitude = "123";
             deviceInformation.LocationLatitude = "456";
             deviceInformation.DeviceModel = "Samsing";
             */



            getLocation();


            //Toast.MakeText(this, "The Network Provider does not exist or is not enabled!", ToastLength.Long).Show();


            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.MaxResponseContentBufferSize = 256000;

            var obj = JsonConvert.SerializeObject(viktem, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/victim/?pns=gcm&to_tag=" + viktem.FBID + "T");
            request.Content = new StringContent(obj, System.Text.Encoding.UTF8, "application/json");

            var data = client.SendAsync(request).Result;


        }


    }
}