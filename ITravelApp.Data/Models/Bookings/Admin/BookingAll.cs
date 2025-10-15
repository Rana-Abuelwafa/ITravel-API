using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.Bookings.Admin
{
    public class BookingAll
    {
        public int totalPages { get; set; }
        public List<bookingwithdetail>? bookings { get; set; }
    
    }
}
