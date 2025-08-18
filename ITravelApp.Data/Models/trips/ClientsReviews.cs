using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class ClientsReviews : tbl_review
    {
        public string? client_name { get; set; }
        public string? client_img { get; set; }
        public string? client_country { get; set; }
        public string? entry_dateStr { get; set; }
    }
    public class ClientsReviewsResponse
    {
        public int totalPages { get; set; }
        public decimal? average_review_rate { get; set; } 
        public List<ClientsReviews> reviews { get; set; }
    }
}
