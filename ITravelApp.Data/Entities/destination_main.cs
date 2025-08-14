using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class destination_main
{
    public int id { get; set; }

    public string? dest_default_name { get; set; }

    public string? dest_code { get; set; }

    public bool? active { get; set; }

    public string? country_code { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }

    public string? route { get; set; }
}
