﻿using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class TripsAll : tripwithdetail
    {
        public int? total_reviews {  get; set; }
        public decimal? review_rate { get; set; }
        public List<TripFacility> facilities { get; set; }
        public List<trip_img> imgs {  get; set; }
    }
    public class TripFacility
    {
        public string? facility_name { get; set; }

        public string? facility_desc { get; set; }
    }
}
