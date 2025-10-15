using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class newsletter_subscriber
{
    public string client_email { get; set; } = null!;

    public string? client_name { get; set; }

    public bool? is_confirmed { get; set; }

    public DateTime? subscribed_at { get; set; }

    public string? language_code { get; set; }

    public string? client_id { get; set; }

    public int id { get; set; }
}
