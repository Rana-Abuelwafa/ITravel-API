using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_main
{
    public string? trip_code { get; set; }

    public string? trip_default_name { get; set; }

    public bool? active { get; set; }

    public string? trip_duration { get; set; }

    public string? pickup { get; set; }

    public bool? show_in_top { get; set; }

    public bool? show_in_slider { get; set; }

    public int destination_id { get; set; }

    public long id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }

    public string? route { get; set; }

    public int? trip_type { get; set; }
}
