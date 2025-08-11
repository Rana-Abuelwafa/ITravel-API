using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trips_booking
{
    public long id { get; set; }

    public long? trip_id { get; set; }

    public string? client_id { get; set; }

    public string? client_email { get; set; }

    public int? total_pax { get; set; }

    public string? booking_code { get; set; }

    public DateTime? booking_date { get; set; }

    public int? child_num { get; set; }

    public decimal? total_price { get; set; }

    public string? pickup_time { get; set; }

    /// <summary>
    /// 1 = requested
    /// 2 = confirmed
    /// 3 = canceled
    /// </summary>
    public int? booking_status { get; set; }

    public DateTime? trip_date { get; set; }

    public string? booking_notes { get; set; }

    public string? trip_code { get; set; }
}
