﻿using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.destination
{
    public class DestinationResponse : destination_translation
    {
        public string? dest_code { get; set; }
        public string? country_code { get; set; }
        public string? img_path { get; set; }
    }
}
