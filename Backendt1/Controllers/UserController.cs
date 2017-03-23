using Backendt1.DataObjects;
using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.Mobile.Server;

namespace Backendt1.Controllers
{

    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class UserController : ApiController
    {
        // GET: api/Person
        public ArrayList Get()
        {
            UserPersistence pp = new UserPersistence();
            return pp.getUsers();
        }

        // GET: api/Person/5
        public User Get(String id)
        {
            UserPersistence pp = new UserPersistence();
            User p = pp.getUser(id);
            if (p == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return p;
        }
        //"api/{controller}/{id}"
        /*[Route("api/search/{text}")]
        [HttpGet]*/
        [HttpGet, Route("api/search/{text}")]
        public ArrayList GetSearch(string text)
        {
            UserPersistence pp = new UserPersistence();
            var p = pp.searchUser(text);
            if (p == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return p;
        }

        // POST: api/Person
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post([FromBody]User value)
        {

            UserPersistence pp = new UserPersistence();
            String id;
            id = pp.saveUser(value);

            /*// Get the settings for the server project.
            HttpConfiguration config = this.Configuration;
            MobileAppSettingsDictionary settings =
                this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["messageParam"] = value.UserName + " was added to the list.";

            try
            {
                // Send the push notification and log the results.
                var result = await hub.SendTemplateNotificationAsync(templateParams);

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
            }*/

            value.FBID = id;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("/user/{0}", id));
            return response;

        }

        // PUT: api/Person/5
        public HttpResponseMessage Put(string id, [FromBody]User value)
        {
            UserPersistence pp = new UserPersistence();
            bool recordExisted = false;
            recordExisted = pp.updateUser(id, value);


            HttpResponseMessage response;

            if (recordExisted)
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);

            }
            return response;

        }

        // DELETE: api/Person/5
        public HttpResponseMessage Delete(string id)
        {
            UserPersistence pp = new UserPersistence();
            bool recordExisted = false;
            recordExisted = pp.deleteUser(id);

            HttpResponseMessage response;

            if (recordExisted)
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);

            }
            return response;
        }
    }
}
