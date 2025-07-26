using ITravelApp.Data.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.trips;
using Microsoft.EntityFrameworkCore;
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
                                 img_path = combined != null ? "https://api.waslaa.de//" + combined.img_path : null,
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

        #region trips

        public List<TripFacility> getFacilityForTrip(decimal? trip_id, string lang_code)
        {
            try
            {
                var result = from TFAC in _db.trip_facilities.Where(wr => wr.trip_id == trip_id)
                             join TRANS in _db.facility_translations.Where(wr => wr.lang_code == lang_code) on TFAC.facility_id equals TRANS.facility_id into TRIPFAC
                             from m in TRIPFAC.DefaultIfEmpty()
                             select new TripFacility
                             {
                                 facility_desc = m.facility_desc,
                                 facility_name = m.facility_name,

                             };
                return result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<trip_img>> GetImgsByTrip(decimal? trip_id)
        {
            try
            {
                return await _db.trip_imgs.Where(wr => wr.trip_id == trip_id).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<TripsAll>> GetTripsAll(TripsReq req)
        {
            try
            {
                var trips = await _db.tripwithdetails
                    .Where(wr => wr.lang_code == req.lang_code && 
                                 wr.show_in_top == (req.show_in_top == false ? wr.show_in_top : req.show_in_top) && 
                                 wr.destination_id == (req.destination_id == 0 ? wr.destination_id : req.destination_id) &&
                                 wr.currency_code ==req.currency_code)
                    .ToListAsync();
                    return trips.Select(s => new TripsAll
                    {
                        destination_id = s.destination_id,
                        lang_code = s.lang_code,
                        country_code = s.country_code,
                        currency_code = s.currency_code,
                        default_img = s.default_img,
                        dest_code = s.dest_code,
                        dest_default_name = s.dest_default_name,
                        pickup = s.pickup,
                        show_in_slider = s.show_in_slider,
                        show_in_top = s.show_in_top,
                        trip_code = s.trip_code,
                        trip_default_name = s.trip_default_name,
                        trip_description = s.trip_description,
                        trip_duration = s.trip_duration,
                        trip_highlight = s.trip_highlight,
                        trip_id = s.trip_id,
                        trip_includes = s.trip_includes,
                        trip_name = s.trip_name,
                        trip_origin_price = s.trip_origin_price,
                        trip_sale_price = s.trip_sale_price,
                        trip_trans_id = s.trip_trans_id,
                        facilities = getFacilityForTrip(s.trip_id, s.lang_code).ToList(),
                        imgs = GetImgsByTrip(s.trip_id).Result
                    })
                    .ToList();


               

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req)
        {
            try
            {
                var trips = await _db.tripwithdetails
                    .Where(wr => wr.lang_code == req.lang_code && 
                                wr.show_in_slider == true && 
                                wr.destination_id == (req.destination_id == 0 ? wr.destination_id : req.destination_id) &&
                                wr.currency_code == req.currency_code)
                    .ToListAsync();


                return trips;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
