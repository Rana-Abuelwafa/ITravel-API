using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings
{
    public class CalculateBookingPriceReq
    {
        public long? booking_id { get; set; }
        public long? trip_id { get; set; }
        public int? adult_num { get; set; }
        public int? child_num { get; set; }
        public string? currency_code { get; set; }

        public List<ExtraWithPrice>? extra_lst { get; set; }
        public List<ExtraWithPrice>? extra_obligatory { get; set; }
        public List<int>? childAges { get; set; } = new();
        public bool? is_two_way { get; set; } = false;
        //public decimal? extras_price { get; set; }
    }
    public class ExtraWithPrice {
        public decimal? extra_price { get; set; }
        public int? extra_count { get; set; }
        public int? pricing_type { get; set; }
    }
}
