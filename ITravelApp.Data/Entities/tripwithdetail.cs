using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class tripwithdetail
{
    public long? trip_trans_id { get; set; }

    public string? trip_description { get; set; }

    public string? trip_includes { get; set; }

    public string? lang_code { get; set; }

    public string? trip_highlight { get; set; }

    public long? trip_id { get; set; }

    public string? trip_name { get; set; }

    public int? destination_id { get; set; }

    public string? pickup { get; set; }

    public bool? show_in_slider { get; set; }

    public bool? show_in_top { get; set; }

    public string? trip_code { get; set; }

    public string? trip_default_name { get; set; }

    public string? trip_duration { get; set; }

    public string? route { get; set; }

    public string? default_img { get; set; }

    public string? dest_default_name { get; set; }

    public string? dest_code { get; set; }

    public string? country_code { get; set; }

    public string? dest_route { get; set; }

    public int? trip_type { get; set; }

    public string? trip_not_includes { get; set; }

    public string? important_info { get; set; }

    public string? trip_details { get; set; }

    public string? trip_category_name { get; set; }

    public string? trip_category_code { get; set; }

    public string? trip_code_auto { get; set; }

    public string? cancelation_policy { get; set; }

    public int? release_days { get; set; }

    public int? trip_order { get; set; }

    public bool? is_comm_soon { get; set; }
}
