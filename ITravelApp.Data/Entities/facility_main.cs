using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class facility_main
{
    public int id { get; set; }

    public string? facility_code { get; set; }

    public string? facility_default_name { get; set; }

    public bool? active { get; set; }
}
