using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.NotificationHubs;

namespace Backendt1.Models
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            //Endpoint=sb://simplerestserver.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=l7ZJ9WGYLaQ7jzwmtsLF0pdwSY8KmVmOVzrbHxFMkBw=
            //"<your hub's DefaultFullSharedAccessSignature>","<hub name>"
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://simplerestserver.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=l7ZJ9WGYLaQ7jzwmtsLF0pdwSY8KmVmOVzrbHxFMkBw=", "sbanotifi");
        }
    }
}