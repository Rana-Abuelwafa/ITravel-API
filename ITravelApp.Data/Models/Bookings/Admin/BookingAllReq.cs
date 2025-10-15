using ITravelApp.Data.Models.global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings.Admin
{
    public class BookingAllReq : PaginationReq
    {
        public string? client_email { get; set; }
        public string? booking_code { get; set; }
        public long? trip_id { get; set; }
        public string? date_from { get; set; }
        public string? date_to { get; set; }
    }
}
