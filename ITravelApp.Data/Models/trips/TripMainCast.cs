using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripMainCast : trip_main
    {
         public string? dest_default_name { get; set; }

        public string? dest_code { get; set; }

        public string? country_code { get; set; }
    }
}
