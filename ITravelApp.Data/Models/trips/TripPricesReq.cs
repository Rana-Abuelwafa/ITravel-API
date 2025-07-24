using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripPricesReq : trip_price
    {
        public bool delete { get; set; }
    }
}
