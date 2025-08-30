using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_facility
{
    public long? trip_id { get; set; }

    public long? facility_id { get; set; }

    public bool? active { get; set; }

    public long id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }

    public int? trip_type { get; set; }
}
