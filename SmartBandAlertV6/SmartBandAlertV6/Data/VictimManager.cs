using SmartBandAlertV6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV6.Data
{
    public class VictimManager
    {

        IRestService restService;



        public VictimManager(IRestService service)

        {

            restService = service;

        }

        public Task SaveTaskAsync(Victim item, bool isNewItem = false)

        {

            return restService.SaveVictimAsync(item, isNewItem);


        }

        public Task<Victim> SearchVictimAsync(string text)

        {

            return restService.SearchVictimAsync(text);

        }



    }
}
