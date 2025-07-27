using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_pickups_translation
{
    public long trip_pickup_id { get; set; }

    public string? pickup_name { get; set; }

    public string? pickup_description { get; set; }

    public string? lang_code { get; set; }

    public long id { get; set; }
}
