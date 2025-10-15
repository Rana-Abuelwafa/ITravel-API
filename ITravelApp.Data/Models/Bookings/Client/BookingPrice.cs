using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings.Client
{
    public class BookingPrice
    {
        public decimal? total_adult_price { get; set; }
        public decimal? total_child_price { get; set; }
        public decimal? final_price { get; set; }
        public decimal? optional_extras_price { get; set; }
        public decimal? opligatory_extras_price { get; set; }
        public string? message { get; set;}
        public bool? success { get; set; }
    }
}
