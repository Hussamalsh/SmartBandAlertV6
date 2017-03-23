using Backendt1.DataObjects;
using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Backendt1.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class FriendsController : ApiController
    {
        // GET: api/Person
        public ArrayList Get()
        {
            FriendsPersistence pp = new FriendsPersistence();
            return pp.getFriends();
        }

        // GET: api/Person/5
        public ArrayList Get(String id)
        {
            FriendsPersistence pp = new FriendsPersistence();
            /*Friends p = pp.getFriends(id);
            if (p == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }*/
            return pp.getFriends(id);
        }

        // POST: api/friends
        public HttpResponseMessage Post([FromBody]Friends value)
        {

            FriendsPersistence pp = new FriendsPersistence();
            String id;
            id = pp.saveFriend(value);




            value.UserFBID = id;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("/friends/{0}", id));
            return response;

        }

        // PUT: api/Person/5
        public HttpResponseMessage Put(string id, [FromBody]Friends value)
        {
            FriendsPersistence pp = new FriendsPersistence();
            bool recordExisted = false;
            recordExisted = pp.updateFriend(id, value);


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
        //"api/{controller}/{id}"
        [Route("api/friends/{userId}/{friendId}")]
        [HttpDelete]
        public HttpResponseMessage Delete(string userId, string friendId)
        {
            FriendsPersistence pp = new FriendsPersistence();
            bool recordExisted = false;
            recordExisted = pp.deleteFriend(userId, friendId);

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
