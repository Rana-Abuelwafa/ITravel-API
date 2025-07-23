using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class destination_img
{
    public int id { get; set; }

    public decimal? destination_id { get; set; }

    public string? img_path { get; set; }

    public string? img_name { get; set; }

    public bool? is_default { get; set; }
}
