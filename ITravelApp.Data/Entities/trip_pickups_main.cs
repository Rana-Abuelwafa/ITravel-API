using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_pickups_main
{
    public long? trip_id { get; set; }

    /// <summary>
    /// 1 = exercusion trip
    /// 2 = transfer trip
    /// </summary>
    public short? trip_type { get; set; }

    public int? order { get; set; }

    public string? pickup_code { get; set; }

    public string? pickup_default_name { get; set; }

    public string? duration { get; set; }

    public long id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }
}
