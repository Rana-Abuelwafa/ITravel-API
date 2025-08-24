using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripsPickupResponseGrp
    {
        public long? trip_id { get; set; }
        public short? trip_type { get; set; }

        public int? order { get; set; }
        public string? duration { get; set; }
        public string? pickup_code { get; set; }

        public string? pickup_default_name { get; set; }
        public decimal? trip_pickup_id { get; set; }
        public List<TripsPickupResponse> translations { get; set; }
    }
}
