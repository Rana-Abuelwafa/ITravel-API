using ITravelApp.Data.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.destination;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data
{
    public class ClientDAO
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly itravel_client_dbContext _db;

        public ClientDAO(itravel_client_dbContext db, IStringLocalizer<Messages> localizer)
        {
            _db = db;
            _localizer = localizer;
        }
        #region destination

        public List<DestinationResponse> getDestinations(DestinationReq req)
        {
            try
            {

                var result = from trans in _db.destination_translations.Where(wr => wr.lang_code == req.lang_code && wr.active == true)
                             join dest in _db.destination_mains.Where(wr => wr.active == true && wr.country_code.ToLower() == (String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower())) on trans.destination_id equals dest.id         // INNER JOIN
                             join img in _db.destination_imgs on trans.id equals img.destination_id into DestAll 
                             from combined in DestAll.DefaultIfEmpty()               // LEFT JOIN
                             select new DestinationResponse
                             {
                                 destination_id = trans.destination_id,
                                 id = trans.id,
                                 country_code = dest.country_code,
                                 active = dest.active,
                                 dest_code = dest.dest_code,
                                 dest_description = trans.dest_description,
                                 dest_name = trans.dest_name,
                                 img_path = combined !=null ? "https://api.waslaa.de//" + combined.img_path :  null,
                                 lang_code = trans.lang_code


                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
