using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backendt1.DataObjects
{
    public class Victim
    {
        public String FBID { get; set; }

        public String UserName { get; set; }

        public DateTime StartDate { get; set; }

        public String Latitude { get; set; }

        public String Longitude { get; set; }
        public String Adress { get; set; }


    }
}