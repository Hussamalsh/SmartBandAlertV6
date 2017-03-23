using Microsoft.WindowsAzure.MobileServices;
using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public class FriendsManager
    {
        IRestService restService;

      

        public FriendsManager(IRestService service)
        {

            restService = service;

        }



        public Task<List<FriendsList>> GetTasksAsync()
        {

            return restService.RefreshDataAsyncFriends();

        }



        public Task SaveTaskAsync(FriendsList item, bool isNewItem = false)
        {

            return restService.SaveTodoItemAsyncFriend(item, isNewItem);

        }



        public Task DeleteTaskAsync(FriendsList item)
        {

            return restService.DeleteTodoItemAsync(App.FacebookId, item.FriendFBID);

        }


    }
}
