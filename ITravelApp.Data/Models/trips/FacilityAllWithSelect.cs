using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.trips
{
    public class FacilityAllWithSelect : facility_main
    {
        public bool selected { get; set; }
    }
}
