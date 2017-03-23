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
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SmartBandAlertV6.Droid.Services
{
    public class RegisterClient
    {


        HttpClient client;


        public RegisterClient()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.MaxResponseContentBufferSize = 256000;
        }


        private class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        //POST_URL = backendEndpoint + "/api/register";
        private string POST_URL = "http://sbat1.azurewebsites.net/api/register";

        public async Task RegisterAsync(string handle, IEnumerable<string> tags)
        {
            var regId = await RetrieveRegistrationIdOrRequestNewOneAsync(handle);

            var deviceRegistration = new DeviceRegistration
            {
                Platform = "gcm",
                Handle = handle,
                Tags = tags.ToArray<string>(),
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