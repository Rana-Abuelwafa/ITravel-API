using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.profile
{
    public class ClientProfileCast : client_Profile
    {
        [NotMapped]
        public string? client_birthdayStr { get; set; }
    }
}
