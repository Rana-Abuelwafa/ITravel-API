using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_img
{
    public long? trip_id { get; set; }

    public string? img_path { get; set; }

    public string? img_name { get; set; }

    public bool? is_default { get; set; }

    public long id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }

    public string? img_resize_path { get; set; }

    public int? img_width { get; set; }

    public int? img_height { get; set; }

    public int? trip_type { get; set; }

    public int? img_order { get; set; }
}
