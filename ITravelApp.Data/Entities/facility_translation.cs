using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class facility_translation
{
    public long? facility_id { get; set; }

    public string? lang_code { get; set; }

    public string? facility_name { get; set; }

    public string? facility_desc { get; set; }

    public long id { get; set; }
}
