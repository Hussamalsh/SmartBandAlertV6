using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartBandAlertV6.iOS.Services
{
    public class iOSLongRunningTaskExample
    {
        nint _taskId;
        CancellationTokenSource _cts;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();

            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("LongRunningTask", OnExpiration);

            try
            {
                //INVOKE THE SHARED CODE
                //var counter = new TaskCounter();
                await RunCounter(_cts.Token);

            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_cts.IsCancellationRequested)
                {
                    //var message = new CancelledMessage();
                    Device.BeginInvokeOnMainThread(
                       () => MessagingCenter.Send("message", "CancelledMessage")
                    );
                }
            }

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        void OnExpiration()
        {
            _cts.Cancel();
        }


        public async Task RunCounter(CancellationToken token)
        {
            await Task.Run(async () => {

                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    /*var message = new TickedMessage
                    {
                        Message = i.ToString()
                    };*/

                    Device.BeginInvokeOnMainThread(() => {
                        MessagingCenter.Send("message", "TickedMessage");
                        notify();
                    });

                    await Task.Delay(61000);



                }
            }, token);
        }


        public void notify()
        {
            // create the notification
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(0);

            // configure the alert
            notification.AlertAction = "View Alert";
            notification.AlertBody = "Your one minute alert has fired!";

            // modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }
    }
}