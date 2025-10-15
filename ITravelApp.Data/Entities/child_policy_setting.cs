using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class child_policy_setting
{
    public string? code_auto { get; set; }

    public int? age_from { get; set; }

    public int? age_to { get; set; }

    public decimal? child_price { get; set; }

    /// <summary>
    /// 1 =Free
    /// 2=% of Adult Price
    /// 3=Fixed Amount
    /// </summary>
    public int? pricing_type { get; set; }

    public string? notes { get; set; }

    public long? trip_id { get; set; }

    public DateTime? created_at { get; set; }

    public string? created_by { get; set; }

    public DateTime? updated_at { get; set; }

    public string? currency_code { get; set; }

    public int policy_id { get; set; }
}
