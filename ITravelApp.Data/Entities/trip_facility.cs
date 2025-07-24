using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_facility
{
    public int id { get; set; }

    public decimal? trip_id { get; set; }

    public decimal? facility_id { get; set; }

    public bool? active { get; set; }
}
