using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.destination
{
    public class DestinationImgUpdateReq : destination_img
    {
        public bool delete { get; set; }
    }
}
