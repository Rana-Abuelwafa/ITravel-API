using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripsWishlistReq : trips_wishlist
    {
        public bool? delete { get; set; }
    }
}
