using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripsPickupResponse
    {
        public long? trip_id { get; set; }

        /// <summary>
        /// 1 = exercusion trip
        /// 2 = transfer trip
        /// </summary>
        public short? trip_type { get; set; }

        public int? order { get; set; }

        public string? pickup_code { get; set; }

        public string? pickup_default_name { get; set; }
        public decimal? trip_pickup_id { get; set; }

        public string? pickup_name { get; set; }

        public string? pickup_description { get; set; }

        public string? lang_code { get; set; }
        public string? duration { get; set; }
    }
}
