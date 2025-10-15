using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.destination
{
    public class DestinationWithTranslationsPag
    {
        public int totalPages { get; set; }
        public List<DestinationWithTranslations>? result { get; set; }
    }
}
