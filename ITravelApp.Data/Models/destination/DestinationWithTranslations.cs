using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.destination
{
    public class DestinationWithTranslations 
    {
        public int? destination_id { get; set; }
        public string? dest_default_name { get; set; }
        public string? dest_code { get; set; }
        public string? country_code { get; set; }
        public string? img_path { get; set; }
        public string? route { get; set; }
        public bool? active { get; set; }
        public int? parent_id { get; set; }
        public bool? leaf { get; set; }
        public string? parent_name { get; set; }
        public int? order { get; set; }
        public List<DestinationResponse> translations { get; set; }
        //public List<destination_img> images { get; set; }
    }
}
