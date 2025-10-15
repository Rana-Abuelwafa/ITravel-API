using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class booking_extra
{
    public long? booking_id { get; set; }

    public int? extra_id { get; set; }

    public DateTime? created_at { get; set; }

    public string? created_by { get; set; }

    public DateTime? updated_at { get; set; }

    public int? extra_count { get; set; }

    public int id { get; set; }
}
