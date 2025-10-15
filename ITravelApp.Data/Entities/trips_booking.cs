using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trips_booking
{
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

    public int? infant_num { get; set; }

    public string? pickup_address { get; set; }

    public string? client_phone { get; set; }

    public string? booking_code_auto { get; set; }

    public string? client_nationality { get; set; }

    public string? gift_code { get; set; }

    public int? trip_type { get; set; }

    public string? currency_code { get; set; }

    public long id { get; set; }

    public string? client_name { get; set; }

    public bool? is_two_way { get; set; }

    public DateTime? trip_return_date { get; set; }

    public string? child_ages { get; set; }

    public short? pricing_type { get; set; }
}
