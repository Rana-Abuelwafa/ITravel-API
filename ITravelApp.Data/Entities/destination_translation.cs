using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class destination_translation
{
    public int id { get; set; }

    public int? destination_id { get; set; }

    public string? lang_code { get; set; }

    public string? dest_name { get; set; }

    public string? dest_description { get; set; }

    public bool? active { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }
}
