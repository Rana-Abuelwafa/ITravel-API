using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.destination
{
    public class DestinationResponse : destination_translation
    {
        public string? dest_default_name { get; set; }
        public string? dest_code { get; set; }
        public string? country_code { get; set; }
        public string? img_path { get; set; }
        public string? route { get; set; }
        public int? parent_id { get; set; }
        public int? order { get; set; }
        public string? parent_name { get; set; }
        public int? parent_order { get; set; }
        public bool? leaf { get; set; }
        public int? trip_type { get; set; }
    }
}
