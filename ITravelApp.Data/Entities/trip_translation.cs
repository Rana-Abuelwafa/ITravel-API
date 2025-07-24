using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_translation
{
    public int id { get; set; }

    public decimal? trip_id { get; set; }

    public string? lang_code { get; set; }

    public string? trip_name { get; set; }

    public string? trip_description { get; set; }

    public string? trip_includes { get; set; }

    public string? trip_highlight { get; set; }
}
