using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class destinationwithdetail
{
    public bool? trans_active { get; set; }

    public string? dest_description { get; set; }

    public string? dest_name { get; set; }

    public int? destination_id { get; set; }

    public int? id { get; set; }

    public string? lang_code { get; set; }

    public int? order { get; set; }

    public bool? active { get; set; }

    public string? country_code { get; set; }

    public string? dest_code { get; set; }

    public string? dest_default_name { get; set; }

    public bool? leaf { get; set; }

    public int? parent_id { get; set; }

    public string? route { get; set; }

    public string? img_name { get; set; }

    public string? img_path { get; set; }

    public bool? is_default { get; set; }
}
