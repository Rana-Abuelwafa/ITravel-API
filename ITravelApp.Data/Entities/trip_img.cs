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
}
