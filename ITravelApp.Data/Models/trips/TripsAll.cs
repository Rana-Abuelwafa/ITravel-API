using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripsAll : tripwithdetail
    {
        public long wish_id { get; set; }
        public string? client_id { get; set; }
        public string? wsh_created_at { get; set; }
        public bool? isfavourite {  get; set; }
        public int? total_reviews {  get; set; }
        public decimal? review_rate { get; set; }
        public decimal? trip_min_price { get; set; }
        public decimal? trip_max_price { get; set; }
        public int? trip_max_capacity { get; set; }
        public string currency_code { get; set; }
        public List<TripFacility> facilities { get; set; }
        public List<trip_img> imgs {  get; set; }
        public int? max_child_age { get; set; }
        public string? pricing_type { get; set; }
        public int? pricing_type_id { get; set; }
        public List<Child_Prices> child_lst { get; set; } = new List<Child_Prices>();
    }
    public class TripFacility
    {
        public string? facility_name { get; set; }

        public string? facility_desc { get; set; }
        public bool? is_extra { get; set; }

        public decimal? extra_price { get; set; }
        public short? pricing_type { get; set; }
        public bool? is_obligatory { get; set; }
        public string? currency_code { get; set; }
        public long? facility_id { get; set; }
    }
    public class Child_Prices
    {
        public int? age_from { get; set; }

        public int? age_to { get; set; }

        public decimal? child_price { get; set; }
    }
}
