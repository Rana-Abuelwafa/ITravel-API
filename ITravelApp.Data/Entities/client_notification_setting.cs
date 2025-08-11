using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class client_notification_setting
{
    public string? client_id { get; set; }

    public bool? review_reminder { get; set; }

    public bool? recommendation_reminder { get; set; }

    public bool? deals_reminder { get; set; }

    public bool? travel_guide_reminder { get; set; }

    public long id { get; set; }

    public decimal? profile_id { get; set; }
}
