using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_translation
{
    public long? trip_id { get; set; }

    public string? lang_code { get; set; }

    public string? trip_name { get; set; }

    public string? trip_description { get; set; }

    public string? trip_includes { get; set; }

    public string? trip_highlight { get; set; }

    public string? trip_details { get; set; }

    public string? important_info { get; set; }

    public long id { get; set; }
}
