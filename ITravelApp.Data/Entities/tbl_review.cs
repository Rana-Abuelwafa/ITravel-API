using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class tbl_review
{
    public long id { get; set; }

    public string? client_id { get; set; }

    public string? review_title { get; set; }

    public string? review_description { get; set; }

    public DateTime? entry_date { get; set; }

    public decimal? review_rate { get; set; }

    public long? trip_id { get; set; }

    /// <summary>
    /// 1 = exercusion trip
    /// 2 = transfer trip
    /// </summary>
    public short? trip_type { get; set; }
}
