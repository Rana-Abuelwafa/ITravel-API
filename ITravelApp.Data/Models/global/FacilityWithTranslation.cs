using ITravelApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data.Models.global
{
    public class FacilityWithTranslation : facility_translation
    {
        public bool? active { get; set; }
        public string? facility_code { get; set; }
        public string? facility_default_name { get; set; }
    }

    public class FacilityWithTranslationGrp
    {
        public bool? active { get; set; }
        public long? facility_id { get; set; }
        public string? facility_code { get; set; }
        public string? facility_default_name { get; set; }
        public List<FacilityWithTranslation> translations { get; set; }
    }
}
