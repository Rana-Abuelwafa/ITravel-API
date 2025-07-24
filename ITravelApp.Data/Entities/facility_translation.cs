using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class facility_translation
{
    public int id { get; set; }

    public decimal? facility_id { get; set; }

    public string? lang_code { get; set; }

    public string? facility_name { get; set; }

    public string? facility_desc { get; set; }
}
