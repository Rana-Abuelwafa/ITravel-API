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

    public bool? is_extra { get; set; }

    public decimal? extra_price { get; set; }

    public string? currency_code { get; set; }

    /// <summary>
    /// 1 = per pax
    /// 2= per unit
    /// </summary>
    public short? pricing_type { get; set; }

    public bool? is_obligatory { get; set; }
}
