using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_category
{
    public int id { get; set; }

    public string? type_name { get; set; }

    public string? type_code { get; set; }
}
