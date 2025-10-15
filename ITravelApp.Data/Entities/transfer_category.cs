using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class transfer_category
{
    public string? category_code { get; set; }

    public decimal? min_price { get; set; }

    public decimal? max_price { get; set; }

    public string? currency_code { get; set; }

    public int? min_capacity { get; set; }

    public int? max_capacity { get; set; }

    public string? category_name { get; set; }

    public string? created_by { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public decimal? child_price { get; set; }

    public string? notes { get; set; }

    public int id { get; set; }
}
