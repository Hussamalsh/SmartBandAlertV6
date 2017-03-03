using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public interface IRestService

    {

        Task<List<User>> RefreshDataAsync();

        Task<List<User>> SearchUsersAsync(string text);

        Task <Victim> SearchVictimAsync(string text);
        Task<List<FriendsList>> RefreshDataAsyncFriends();

        Task SaveTodoItemAsync(User item, bool isNewItem);

        Task SaveVictimAsync(Victim item, bool isNewItem);

        Task DeleteTodoItemAsync(String userid, String friendid);



    }
}
