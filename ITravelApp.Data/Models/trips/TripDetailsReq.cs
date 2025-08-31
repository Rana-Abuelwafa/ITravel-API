using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripDetailsReq
    {
        public long trip_id { get; set; }
        public string? lang_code { get; set; }
        public string? currency_code { get; set; }
        public string? client_id { get; set; }
        public int trip_type { get; set; } = 1;
    }
}
