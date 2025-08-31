using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripsReq
    {
        public int destination_id { get; set; }
        public string? lang_code { get; set; }
        public bool? show_in_top { get; set; }
        public bool? show_in_slider { get; set; }
        public string? currency_code { get; set; }
        public string? client_id { get; set; }
        public int trip_type { get; set; } = 1;
    }
}
