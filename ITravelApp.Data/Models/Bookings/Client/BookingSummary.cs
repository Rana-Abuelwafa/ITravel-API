using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings.Client
{
    public class BookingSummary :bookingwithdetail 
    {
        public List<BookingExtraCast>? extras {  get; set; }
        public List<BookingExtraCast>? extras_obligatory { get; set; }
    }
}
