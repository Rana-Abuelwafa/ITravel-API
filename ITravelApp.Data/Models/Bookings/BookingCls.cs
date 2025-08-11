using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings
{
    public class BookingCls : trips_booking
    {
        public string? booking_dateStr { get; set; }
        public string? trip_dateStr { get; set; }
    }
}
