using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_main
{
    public int id { get; set; }

    public string? trip_code { get; set; }

    public string? trip_default_name { get; set; }

    public bool? active { get; set; }

    public string? trip_duration { get; set; }

    public string? pickup { get; set; }

    public bool? show_in_top { get; set; }

    public bool? show_in_slider { get; set; }

    public decimal? destination_id { get; set; }
}
