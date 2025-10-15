using System;
using System.Collections.Generic;

namespace ITravelApp.Data.Entities;

public partial class client_Profile
{
    public decimal profile_id { get; set; }

    public string client_id { get; set; } = null!;

    public string? client_name { get; set; }

    public string? client_email { get; set; }

    public string? phone_number { get; set; }

    public string? nation { get; set; }

    public string? gender { get; set; }

    public string? lang { get; set; }

    public string? pay_code { get; set; }

    public DateOnly? client_birthday { get; set; }

    public string? address { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public string? created_by { get; set; }

    public string? client_first_name { get; set; }

    public string? client_last_name { get; set; }
}
