using ITravelApp.Data.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ITravelApp.Data
{
    public class AdminDAO
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly itravel_client_dbContext _db;

        public AdminDAO(itravel_client_dbContext db, IStringLocalizer<Messages> localizer)
        {
            _db = db;
            _localizer = localizer;
        }

        #region destination

        //save main destination data by admin
        public ResponseCls SaveMainDestination(destination_main row)
        {
            ResponseCls response;
            int maxId = 0;
            var msg = _localizer["DuplicateData"];
            try
            {

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.destination_mains.Where(wr => wr.dest_code == row.dest_code && wr.active == row.active).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.destination_mains.Count() > 0)
                    {
                        maxId = _db.destination_mains.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.destination_mains.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.destination_mains.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }

            return response;
        }
        //save destination's translations
        public ResponseCls SaveDestinationTranslations(destination_translation row)
        {
            ResponseCls response;
            int maxId = 0;
            try
            {

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.destination_translations.Where(wr => wr.dest_name == row.dest_name && wr.destination_id == row.destination_id && wr.active == row.active && wr.lang_code == row.lang_code).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.destination_translations.Count() > 0)
                    {
                        maxId = _db.destination_translations.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.destination_translations.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.destination_translations.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };
            }

            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }

            return response;
        }
        //save destination images
        public ResponseCls saveDestinationImage(destination_img row)
        {
            ResponseCls response;
            int maxId = 0;
            try
            {

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.destination_imgs.Where(wr => wr.destination_id == row.destination_id && wr.is_default == (row.is_default == true ? row.is_default : null)).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.destination_imgs.Count() > 0)
                    {
                        maxId = _db.destination_imgs.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.destination_imgs.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.destination_imgs.Update(row);
                    _db.SaveChanges();
                }


                response = new ResponseCls { errors = null, success = true, idOut = row.id };

            }

            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        public List<DestinationWithTranslations> GetDestinationWithTranslations(DestinationReq req)
        {
            try
            {
                var result =
                    from dest in _db.destination_mains.Where(wr => wr.active == true && wr.country_code.ToLower() == (String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower()))
                    join trans in _db.destination_translations.Where(wr => wr.active == true)
                        on dest.id equals trans.destination_id
                             into dest_trans

                    from combinedDEST in dest_trans.DefaultIfEmpty() // LEFT JOIN Customers
                    join img in _db.destination_imgs.Where(wr => wr.is_default == true)

                       on dest.id equals img.destination_id into DestAll
                    from IMGDEST in DestAll.DefaultIfEmpty() // LEFT JOIN Payments
                    select new DestinationResponse
                    {
                        destination_id = dest.id,
                        id = combinedDEST != null ? combinedDEST.id : 0,
                        country_code = dest.country_code,
                        active = dest.active,
                        dest_code = dest.dest_code,
                        dest_description = combinedDEST != null ? combinedDEST.dest_description : null,
                        dest_name = combinedDEST != null ? combinedDEST.dest_name : null,
                        img_path = IMGDEST != null ? "http://api.raccoon24.de/" + IMGDEST.img_path : null,
                        lang_code = combinedDEST != null ? combinedDEST.lang_code : null,
                        dest_default_name = dest.dest_default_name,
                        route = dest.route
                    };

                //var result = from trans in _db.destination_translations.Where(wr => wr.active == true)
                //             join dest in _db.destination_mains.Where(wr => wr.active == true && wr.country_code.ToLower() == (String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower())) on trans.destination_id equals dest.id         // INNER JOIN
                //             join img in _db.destination_imgs on dest.id equals img.destination_id into DestAll
                //             from combined in DestAll.DefaultIfEmpty()               // LEFT JOIN
                //             select new DestinationResponse
                //             {
                //                 destination_id = trans.destination_id,
                //                 id = trans.id,
                //                 country_code = dest.country_code,
                //                 active = dest.active,
                //                 dest_code = dest.dest_code,
                //                 dest_description = trans.dest_description,
                //                 dest_name = trans.dest_name,
                //                 img_path = combined != null ? "http://api.raccoon24.de/" + combined.img_path : null,
                //                 lang_code = trans.lang_code,
                //                 dest_default_name= dest.dest_default_name    ,
                //                 route=dest.route
                //             };
                return result.ToList().GroupBy(grp => new
                {
                    grp.dest_code,
                    grp.img_path,
                    grp.destination_id,
                    grp.country_code,
                    grp.dest_default_name,
                    grp.route,
                    grp.active

                }).Select(s => new DestinationWithTranslations
                {
                    country_code = s.Key.country_code,
                    dest_code = s.Key.dest_code,
                    img_path = s.Key.img_path,
                    destination_id = s.Key.destination_id,
                    dest_default_name = s.Key.dest_default_name,
                    route = s.Key.route,
                    active = s.Key.active,
                    translations = result.Where(wr => wr.dest_code == s.Key.dest_code).ToList()

                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //get images list for specific trip
        public async Task<List<destination_img>> GetImgsByDestination(int? destination_id)
        {
            try
            {
                return await _db.destination_imgs.Where(wr => wr.destination_id == destination_id).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<destination_main>> GetDestination_Mains()
        {
            return await _db.destination_mains.Where(wr => wr.active == true).ToListAsync();
        }
        #endregion


        #region trips

        //save main trip data by admin
        public ResponseCls SaveMainTrip(trip_main row)
        {
            ResponseCls response;
            long maxId = 0;
            var msg = _localizer["DuplicateData"];
            try
            {
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trip_mains.Where(wr => wr.trip_code == row.trip_code && wr.active == row.active && wr.destination_id == row.destination_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trip_mains.Count() > 0)
                    {
                        maxId = _db.trip_mains.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.trip_mains.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.trip_mains.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        //save trip translation data by admin
        public ResponseCls SaveTripTranslation(TripTranslationReq row)
        {
            ResponseCls response;
            long maxId = 0;
            var msg = _localizer["DuplicateData"];
            try
            {
                trip_translation trip = new trip_translation
                {
                    id = row.id,
                    lang_code = row.lang_code,
                    trip_description = row.trip_description,
                    trip_highlight = row.trip_highlight,
                    trip_id = row.trip_id,
                    trip_includes = row.trip_includes,
                    trip_name = row.trip_name,
                    important_info = row.important_info,
                    trip_details = row.trip_details,
                    trip_not_includes = row.trip_not_includes,
                    created_by = row.created_by
                };
                if (row.delete == true)
                {
                    _db.Remove(trip);
                    _db.SaveChanges();
                    return new ResponseCls { errors = null, success = true };
                }

                if (trip.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trip_translations.Where(wr => wr.lang_code == trip.lang_code && wr.trip_id == trip.trip_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trip_translations.Count() > 0)
                    {
                        maxId = _db.trip_translations.Max(d => d.id);

                    }
                    trip.id = maxId + 1;
                    _db.trip_translations.Add(trip);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.trip_translations.Update(trip);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = trip.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        //assign prices with different currency to trips
        public ResponseCls SaveTripPrices(TripPricesReq row)
        {
            ResponseCls response;
            long maxId = 0;
            try
            {
                trip_price price = new trip_price
                {
                    id = row.id,
                    trip_id = row.trip_id,
                    currency_code = row.currency_code,
                    trip_origin_price = row.trip_origin_price,
                    trip_sale_price = row.trip_sale_price,
                    created_by = row.created_by
                };
                if (row.delete == true)
                {
                    _db.Remove(price);
                    _db.SaveChanges();
                    return new ResponseCls { errors = null, success = true };
                }
                if (price.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trip_prices.Where(wr => wr.trip_id == price.trip_id && wr.currency_code == price.currency_code).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trip_prices.Count() > 0)
                    {
                        maxId = _db.trip_prices.Max(d => d.id);

                    }
                    price.id = maxId + 1;
                    _db.trip_prices.Add(price);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.trip_prices.Update(price);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = price.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        //save trip images
        public ResponseCls saveTripImage(TripImgReq row)
        {
            ResponseCls response;
            long maxId = 0;
            try
            {
                trip_img trip = new trip_img
                {
                    trip_id = row.trip_id,
                    id = row.id,
                    img_name = row.img_name,
                    img_path = row.img_path,
                    is_default = row.is_default,
                    created_by = row.created_by,
                    img_height = row.img_height,
                    img_resize_path = row.img_resize_path,
                    img_width = row.img_width,


                };

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trip_imgs.Where(wr => wr.trip_id == trip.trip_id && wr.is_default == (trip.is_default == true ? trip.is_default : null)).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trip_imgs.Count() > 0)
                    {
                        maxId = _db.trip_imgs.Max(d => d.id);

                    }
                    trip.id = maxId + 1;
                    _db.trip_imgs.Add(trip);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.trip_imgs.Update(trip);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = trip.id };


            }

            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }
        //save main facility data by admin
        public ResponseCls SaveMainFacility(facility_main row)
        {
            ResponseCls response;
            long maxId = 0;
            var msg = _localizer["DuplicateData"];
            try
            {
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.facility_mains.Where(wr => wr.facility_code == row.facility_code && wr.active == row.active).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.facility_mains.Count() > 0)
                    {
                        maxId = _db.facility_mains.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.facility_mains.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.facility_mains.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        //save facility translation data by admin
        public ResponseCls SaveFacilityTranslation(FacilityTranslationReq row)
        {
            ResponseCls response;
            long maxId = 0;
            try
            {
                facility_translation facility = new facility_translation
                {
                    facility_desc = row.facility_desc,
                    facility_id = row.facility_id,
                    facility_name = row.facility_name,
                    id = row.id,
                    lang_code = row.lang_code,
                    created_by = row.created_by
                };
                if (row.delete == true)
                {
                    _db.Remove(facility);
                    _db.SaveChanges();
                    return new ResponseCls { errors = null, success = true };
                }

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.facility_translations.Where(wr => wr.lang_code == facility.lang_code && wr.facility_id == facility.facility_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.facility_translations.Count() > 0)
                    {
                        maxId = _db.facility_translations.Max(d => d.id);

                    }
                    facility.id = maxId + 1;
                    _db.facility_translations.Add(facility);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.facility_translations.Update(facility);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = facility.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        //assign facility to trip  by admin
        public ResponseCls AssignFacilityToTrip(TripFacilityAssignReq row)
        {
            ResponseCls response;
            long maxId = 0;
            try
            {
                trip_facility trip = new trip_facility
                {
                    id = row.id,
                    facility_id = row.facility_id,
                    active = row.active,
                    trip_id = row.trip_id,
                    created_by = row.created_by
                };
                if (row.delete == true)
                {
                    _db.Remove(trip);
                    _db.SaveChanges();
                    return new ResponseCls { errors = null, success = true };
                }

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trip_facilities.Where(wr => wr.trip_id == trip.trip_id && wr.facility_id == trip.facility_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trip_facilities.Count() > 0)
                    {
                        maxId = _db.trip_facilities.Max(d => d.id);

                    }
                    trip.id = maxId + 1;
                    _db.trip_facilities.Add(trip);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.trip_facilities.Update(trip);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = trip.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }


        //save  trip pickups main data by admin
        public ResponseCls SaveMainTripPickups(TripsPickupSaveReq row)
        {
            ResponseCls response;
            long maxId = 0;
            var msg = _localizer["DuplicateData"];
            try
            {
                trip_pickups_main pickup = new trip_pickups_main
                {
                    id = row.id,
                    order = row.order,
                    pickup_code = row.pickup_code,
                    pickup_default_name = row.pickup_default_name,
                    trip_id = row.trip_id,
                    trip_type = row.trip_type,
                    created_by = row.created_by,
                    duration = row.duration
                };
                if (row.delete == true)
                {
                    _db.Remove(pickup);
                    _db.SaveChanges();
                    return new ResponseCls { errors = null, success = true };
                }
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trip_pickups_mains.Where(wr => wr.pickup_code == pickup.pickup_code && wr.trip_id == pickup.trip_id && wr.order == pickup.order).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trip_pickups_mains.Count() > 0)
                    {
                        maxId = _db.trip_pickups_mains.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.trip_pickups_mains.Add(pickup);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.trip_pickups_mains.Update(pickup);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        //save  trip pickups main data by admin
        public ResponseCls SaveTripPickupsTranslations(TripsPickupTranslationSaveReq row)
        {
            ResponseCls response;
            long maxId = 0;
            var msg = _localizer["DuplicateData"];
            try
            {
                trip_pickups_translation pickup = new trip_pickups_translation
                {
                    id = row.id,
                    lang_code = row.lang_code,
                    pickup_description = row.pickup_description,
                    pickup_name = row.pickup_name,
                    trip_pickup_id = row.trip_pickup_id,
                    created_by = row.created_by,

                };
                if (row.delete == true)
                {
                    _db.Remove(pickup);
                    _db.SaveChanges();
                    return new ResponseCls { errors = null, success = true };
                }
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trip_pickups_translations.Where(wr => wr.lang_code == pickup.lang_code && wr.trip_pickup_id == pickup.trip_pickup_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trip_pickups_translations.Count() > 0)
                    {
                        maxId = _db.trip_pickups_translations.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.trip_pickups_translations.Add(pickup);
                    _db.SaveChanges();
                }
                else
                {
                    row.updated_at = DateTime.Now;
                    _db.trip_pickups_translations.Update(pickup);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        public async Task<List<TripMainCast>> GetTrip_Mains(int destination_id)
        {
            try
            {

                return await _db.trip_mains.Where(wr => wr.destination_id == (destination_id == 0 ? wr.destination_id : destination_id))
                      .Join(_db.destination_mains,
                              TRIP => new { TRIP.destination_id },
                              DEST => new { destination_id = DEST.id },
                              (TRIP, DEST) => new TripMainCast
                              {
                                  destination_id = TRIP.destination_id,
                                  active = TRIP.active,
                                  id = TRIP.id,
                                  pickup = TRIP.pickup,
                                  route = TRIP.route,
                                  show_in_slider = TRIP.show_in_slider,
                                  show_in_top = TRIP.show_in_top,
                                  trip_code = TRIP.trip_code,
                                  trip_default_name = TRIP.trip_default_name,
                                  trip_duration = TRIP.trip_duration,
                                  country_code = DEST.country_code,
                                  dest_code = DEST.dest_code,
                                  dest_default_name = DEST.dest_default_name
                              }).ToListAsync();


            }   // return await _db.trip_mains.Where(wr => wr.destination_id == (destination_id == 0 ? wr.destination_id : destination_id)).ToListAsync();
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TripTranslationGrp>> GetTripTranslationGrp(long? trip_id)
        {
            try
            {
                var result = await _db.trip_translations.Where(wr => wr.trip_id == trip_id).ToListAsync();

                List<TripTranslationGrp> translations = new List<TripTranslationGrp>
                        {
                            new TripTranslationGrp { lang_code="en",translation=result.ToList().Where(wr => wr.lang_code == "en").SingleOrDefault() },
                            new TripTranslationGrp { lang_code="de",translation=result.ToList().Where(wr => wr.lang_code == "de").SingleOrDefault() },

                        };
                return translations;
                //if (result.Count > 0)
                //{
                //    return result.GroupBy(g => new
                //    {
                //        g.lang_code
                //    }).Select(s => new TripTranslationGrp
                //    {
                //        lang_code = s.Key.lang_code,
                //        translation = result.ToList().Where(wr => wr.lang_code == s.Key.lang_code).SingleOrDefault()
                //    }).ToList();
                //}
                //else
                //{
                //    List<TripTranslationGrp> translations = new List<TripTranslationGrp>
                //        {
                //            new TripTranslationGrp { lang_code="en",translation=null },
                //            new TripTranslationGrp { lang_code="de",translation=null },

                //        };
                //    return translations;
                //}

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<List<trip_price>> GetTrip_Prices(long? trip_id)
        {
            try
            {
                return await _db.trip_prices.Where(wr => wr.trip_id == trip_id).ToListAsync();
            }
            catch(Exception ex)
            {
                return null;
            }

        }
        
        #endregion

    }
}
