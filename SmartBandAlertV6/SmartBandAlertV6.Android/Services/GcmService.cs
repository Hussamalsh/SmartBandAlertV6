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
using Gcm.Client;
using SmartBandAlertV6.Data;
using Android.Util;
using Android.Support.V4.App;
using Android.Media;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace SmartBandAlertV6.Droid.Services
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        public static string[] SENDER_IDS = new string[] { "1061097097490" };
    }

    [Service]
    public class GcmService : GcmServiceBase
    {
        public static string RegistrationToken { get; private set; }

        public GcmService() : base(PushHandlerBroadcastReceiver.SENDER_IDS) { }
        protected override void OnRegistered(Context context, string registrationToken)
        {
            Log.Verbose("PushHandlerBroadcastReceiver", "GCM Registered: " + registrationToken);
            RegistrationToken = registrationToken;

            var push = TodoItemManager.DefaultManager.CurrentClient.GetPush();
            MainActivity.CurrentActivity.RunOnUiThread(() => Register(push, null));
        }

        protected override void OnUnRegistered(Context context, string registrationToken)
        {
            Log.Error("PushHandlerBroadcastReceiver", "Unregistered RegisterationToken: " + registrationToken);
        }

        protected override void OnError(Context context, string errorId)
        {
            Log.Error("PushHandlerBroadcastReceiver", "GCM Error: " + errorId);
        }

        public async void Register(Microsoft.WindowsAzure.MobileServices.Push push, IEnumerable<string> tags)
        {
            try
            {
                const string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";

                JObject templates = new JObject();
                templates["genericMessage"] = new JObject
                {
                    {"body", templateBodyGCM}
                };

                await push.RegisterAsync(RegistrationToken, templates);
                Log.Info("Push Installation Id", push.InstallationId.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info("PushHandlerBroadcastReceiver", "GCM Message Received!");

            var msg = new StringBuilder();

            if (intent != null && intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
            }

            // Retrieve the message
            var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.PutString("last_msg", msg.ToString());
            edit.Commit();

            string message = intent.Extras.GetString("message");
            if (!string.IsNullOrEmpty(message))
            {
                CreateNotification("New Test!", "SmartBandAlert: " + message);
                App.NotificationOn = true;
                //string tobesearched = "ID = ";
                App.VictimId = message.Split(new[] { "ID =" }, StringSplitOptions.None)[1];
                //message.Substring(message.IndexOf(tobesearched) + tobesearched.Length);//message.("ID ="); ;
                MainActivity._player.Start();
            }
        }

        void CreateNotification(string title, string desc)
        {
            // Create notification
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            // Create an intent to show the UI
            var uiIntent = new Intent(this, typeof(MainActivity));

            // Create the notification
            // we use the pending intent, passing our ui intent over which will get called
            // when the notification is tapped.
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this);
            var notification = builder.SetContentIntent(PendingIntent.GetActivity(this, 0, uiIntent, 0))
                    .SetSmallIcon(Android.Resource.Drawable.SymActionEmail)
                    .SetTicker(title)
                    .SetContentTitle(title)
                    .SetContentText(desc)
                    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification)) // set the sound
                    .SetAutoCancel(true).Build(); // remove the notification once the user touches it

            // Show the notification
            notificationManager.Notify(1, notification);

        }
    }
}