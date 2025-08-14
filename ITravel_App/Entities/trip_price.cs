﻿using System;
using System.Collections.Generic;

namespace ITravel_App.Entities;

public partial class trip_price
{
    public long? trip_id { get; set; }

    public decimal? trip_origin_price { get; set; }

    public decimal? trip_sale_price { get; set; }

    public string? currency_code { get; set; }

    public long id { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }
}
