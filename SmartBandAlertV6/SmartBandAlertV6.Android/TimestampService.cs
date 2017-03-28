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
using Android.Util;
using SmartBandAlertV6.Models;
using System.Collections.ObjectModel;
using System.Threading;
using Plugin.BLE;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System.ComponentModel;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Extensions;
using Android.Media;

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


        IDisposable scan;
        //public bool IsScanning { get; private set; }
        public bool IsScanning => Adapter.IsScanning;

        public Plugin.BLE.Abstractions.Contracts.IAdapter Adapter;
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();

        CancellationTokenSource _cts;


        public Victim viktem;




        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");

            //timestamper = new UtcTimestamper();
            handler = new Handler();

            // This Action is only for demonstration purposes.
            runnable = new Action(() =>
            {
               /* if (timestamper == null)
                {
                    Log.Wtf(TAG, "Why isn't there a Timestamper initialized?");
                }
                else
                {*/
                    string msg = "GetFormattedTimestamp";//timestamper.GetFormattedTimestamp();
                    Log.Debug(TAG, msg);
                    Intent i = new Intent(Constants.NOTIFICATION_BROADCAST_ACTION);
                    i.PutExtra(Constants.BROADCAST_MESSAGE_KEY, msg);
                    Android.Support.V4.Content.LocalBroadcastManager.GetInstance(this).SendBroadcast(i);
                    handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_LOG_MESSAGES);
               // }
            });
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    Log.Info(TAG, "OnStartCommand: The service is already running.");
                }
                else
                {
                    viktem = new Victim();
                    //var ble = CrossBluetoothLE.Current;
                    //Adapter = CrossBluetoothLE.Current.Adapter;
                    Adapter = App.BLEProfileManager.bleprofile.Adapter;
                    /*if (Adapter.ConnectedDevices.Count == 0)
                    {
                        Adapter.DeviceDiscovered += OnDeviceDiscovered;
                        Adapter.StartScanningForDevicesAsync();
                    }
                    else
                    {
                        //var d = Adapter.ConnectedDevices.ToList();

                        foreach (var item in Adapter.ConnectedDevices)
                            Devices.Add(new DeviceListItemViewModel(item));

                       */
                    Devices = App.BLEProfileManager.bleprofile.Devices;
                        checkservice();
                    //}

                    Log.Info(TAG, "OnStartCommand: The service is starting.");
                    RegisterForegroundService();
                    handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_LOG_MESSAGES);
                    isStarted = true;
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                Log.Info(TAG, "OnStartCommand: The service is stopping.");
                //timestamper = null;
                StopForeground(true);
                StopSelf();
                isStarted = false;

            }
            else if (intent.Action.Equals(Constants.ACTION_RESTART_TIMER))
            {
                Log.Info(TAG, "OnStartCommand: Restarting the timer.");
                //timestamper.Restart();

            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
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
            Log.Debug(TAG, /*GetFormattedTimestamp() ??*/ "The TimeStamper has been disposed.");
            Log.Info(TAG, "OnDestroy: The started service is shutting down.");

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

        /// <summary>
        /// ///////////*-------------------------BLE implementations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            AddOrUpdateDevice(args.Device);
        }

        private void AddOrUpdateDevice(IDevice device)
        {


            var vm = Devices.FirstOrDefault(d => d.Device.Id == device.Id);
            if (vm != null)
            {
                vm.Update();
            }
            else
            {
                if (device.Name != null)
                    Devices.Add(new DeviceListItemViewModel(device));
            }

            //this.Devicesl.ItemsSource = this.Devices;
            if (Devices.Count != 0)
            {
                string s = Devices.FirstOrDefault().Device.State.ToString();
                if (s.Equals("Disconnected"))
                {
                    Adapter.ConnectToDeviceAsync(Devices.FirstOrDefault().Device);
                    checkservice();
                }
            }

        }


        /// <summary>
        /// /////////------------------------------------Test
        /// </summary>
        public IList<IService> Services { get; private set; }
        async void checkservice()
        {
            /*foreach (DeviceListItemViewModel item in Devices)
                if (item.Name != null)
                {
                    if (item.Name.Equals("SmartBandAlert"))
                    {
                        IDevice d = item.Device;
                        Adapter.ConnectToDeviceAsync(d);
                        Services = await d.GetServicesAsync();
                        //Services = d.GetServicesAsync();
                        while (Services.Count == 0)
                        {
                            Services = await d.GetServicesAsync();
                        }


                        var service = d.GetServiceAsync(Guid.Parse("ffe0ecd2-3d16-4f8d-90de-e89e7fc396a5"));
                        checkchar();

                    }
                }
                */

            IDevice d = Devices.FirstOrDefault().Device;
            Adapter.ConnectToDeviceAsync(d);

            Services = await d.GetServicesAsync();
            while (Services.Count == 0)
            {
                Services = await d.GetServicesAsync();
            }

            checkchar();

        }
        private IList<ICharacteristic> _characteristics;

        public IList<ICharacteristic> Characteristics
        {
            get { return _characteristics; }
            private set
            {
                _characteristics = value;
                OnPropertyChanged((nameof(_characteristics)));

            }
        }
        async void checkchar()
        {
            foreach (IService item in Services)
                if (item.Name != null)
                {
                    if (item.Name.Equals("Unknown Service"))
                    {
                        Characteristics = new List<ICharacteristic>(await item.GetCharacteristicsAsync());

                    }
                }

            choosechar();
        }
        public ICharacteristic CharacteristicT { get; private set; }

        async void choosechar()
        {

            foreach (ICharacteristic item in Characteristics)
                if (item.Name != null)
                {
                    if (item.Uuid.Equals("6e400003-b5a3-f393-e0a9-e50e24dcca9e"))
                    {
                        CharacteristicT = item;
                        StartUpdates();
                        ReadValueAsync();
                        // WriteValueAsync();
                    }
                }

        }

        private bool _updatesStarted;
        public string UpdateButtonText => _updatesStarted ? "Stop updates" : "Start updates";

        public string Permissions
        {
            get
            {
                if (CharacteristicT == null)
                    return string.Empty;

                return (CharacteristicT.CanRead ? "Read " : "") +
                       (CharacteristicT.CanWrite ? "Write " : "") +
                       (CharacteristicT.CanUpdate ? "Update" : "");
            }
        }
        public string CharacteristicValue => CharacteristicT?.Value.ToHexString().Replace("-", " ");

        private async void WriteValueAsync()
        {
            try
            {


                var data = GetBytes("1");

                await CharacteristicT.WriteAsync(data);

                //RaisePropertyChanged(() => CharacteristicValue);
                OnPropertyChanged((nameof(CharacteristicValue)));
                // DisplayAlert("value", CharacteristicValue, "ok");
                //Messages.Insert(0, $"Wrote value {CharacteristicValue}");
            }
            catch (Exception ex)
            {

            }

        }


        private async void ReadValueAsync()
        {
            if (CharacteristicT == null)
                return;

            try
            {
                //_userDialogs.ShowLoading("Reading characteristic value...");

                CharacteristicT.ReadAsync();

                // RaisePropertyChanged(() => CharacteristicValue);
                OnPropertyChanged(nameof(CharacteristicValue));

                // Messages.Insert(0, $"Read value {CharacteristicValue}");
            }
            catch (Exception ex)
            {
                //_userDialogs.HideLoading();
                // _userDialogs.ShowError(ex.Message);

                //Messages.Insert(0, $"Error {ex.Message}");

            }
            finally
            {
                //_userDialogs.HideLoading();
            }

        }

        private async void StartUpdates()
        {
            try
            {
                _updatesStarted = true;

                CharacteristicT.ValueUpdated -= CharacteristicOnValueUpdated;
                CharacteristicT.ValueUpdated += CharacteristicOnValueUpdated;
                CharacteristicT.StartUpdatesAsync();


                //Messages.Insert(0, $"Start updates");

                //RaisePropertyChanged(() => UpdateButtonText);
                OnPropertyChanged(nameof(UpdateButtonText));
            }
            catch (Exception ex)
            {
                //_userDialogs.ShowError(ex.Message);
            }
        }

        private void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            // Messages.Insert(0, $"Updated value: {CharacteristicValue}");
            //RaisePropertyChanged(() => CharacteristicValue);
            OnPropertyChanged(nameof(CharacteristicValue));

            /*

            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle("Sample Notification")
                .SetContentText("Hello World! This is my first notification!")
                .SetSmallIcon(Resource.Drawable.icon);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);*/


            if (!String.IsNullOrEmpty(CharacteristicValue))
                PostData();




        }

        private static byte[] GetBytes(string text)
        {
            return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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

            /*Geocoder geoCoder = new Geocoder();
            var fortMasonPosition = new Position(position.Latitude, position.Longitude);
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(fortMasonPosition);
            */
            /* = profileManager;
            LoadProfile();*/

            //retreive 
            var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);


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
            var request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/victim/?pns=gcm&to_tag="+ viktem.FBID+ "T");
            request.Content = new StringContent(obj, System.Text.Encoding.UTF8, "application/json");

            var data = client.SendAsync(request).Result;


    }


}
}