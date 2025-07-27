using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class trip_price
{
    public long? trip_id { get; set; }

    public decimal? trip_origin_price { get; set; }

    public decimal? trip_sale_price { get; set; }

    public string? currency_code { get; set; }

    public long id { get; set; }
}
