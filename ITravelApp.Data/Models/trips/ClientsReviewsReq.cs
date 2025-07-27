using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class ClientsReviewsReq
    {
        public long trip_id {  get; set; }
        public int trip_type { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
