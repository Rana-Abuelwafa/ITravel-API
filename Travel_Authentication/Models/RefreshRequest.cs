﻿namespace Travel_Authentication.Models
{
    public class RefreshRequest
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
