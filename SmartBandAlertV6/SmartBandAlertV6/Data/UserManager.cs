using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public class UserManager

    {

        IRestService restService;



        public UserManager(IRestService service)

        {

            restService = service;

        }



        public Task<List<User>> GetTasksAsync()

        {

            return restService.RefreshDataAsync();

        }




        public Task SaveTaskAsync(User item, bool isNewItem = false)

        {

            return restService.SaveTodoItemAsync(item, isNewItem);

        }



        public Task DeleteTaskAsync(User item)

        {

            // return restService.DeleteTodoItemAsync(item.FBID);
            return null;
        }

        public Task<List<User>> SearchUsersAsync(string text)

        {

            return restService.SearchUsersAsync(text);

        }

    }
}
