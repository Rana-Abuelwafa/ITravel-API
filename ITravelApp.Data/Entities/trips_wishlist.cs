using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trips_wishlist
{
    public long id { get; set; }

    public long? trip_id { get; set; }

    public string? client_id { get; set; }

    public DateTime? created_at { get; set; }

    /// <summary>
    /// 1 = exercusion trip
    /// 2 = transfer trip
    /// </summary>
    public short? trip_type { get; set; }
}
