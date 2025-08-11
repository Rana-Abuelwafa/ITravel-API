using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class facility_main
{
    public string? facility_code { get; set; }

    public string? facility_default_name { get; set; }

    public bool? active { get; set; }

    public long id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }
}
