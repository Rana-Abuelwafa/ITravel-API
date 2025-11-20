using ITravelApp.Data.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.Bookings.Client;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
using ITravelApp.Data.Models.trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.Linq;

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

        public async Task<List<destinationwithdetail>> getDestinations(DestinationReq req)
        {
            try
            {
                return await _db.destinationwithdetails.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() &&
                                                                  wr.active == true &&
                                                                  wr.trans_active == true &&
                                                                  wr.leaf == req.leaf &&
                                                                  wr.country_code.ToLower() == (System.String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower())
                                                                  )
                                                                  .OrderBy(x => x.order)
                                                                  .ToListAsync();

                //var result = from trans in _db.destination_translations.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() && wr.active == true)
                //             join dest in _db.destination_mains.Where(wr => wr.active == true && wr.country_code.ToLower() == (System.String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower()) && wr.leaf == req.leaf) on trans.destination_id equals dest.id         // INNER JOIN
                //             join img in _db.destination_imgs on trans.id equals img.destination_id into DestAll
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
                //                 dest_default_name = dest.dest_default_name,
                //                 route = dest.route
                //             };

                //return result.OrderBy(x => x.order).ToList();
            }
            catch (Exception ex)
            {
                return new List<destinationwithdetail>();
            }
        }
        public List<DestinationTree> GetDestination_Tree(DestinationReq req)
        {
            try
            {
                // 1. Get all destination_ids that have trips of the given type
                var tripDestinations = _db.trip_mains
                    .Where(t => t.trip_type == req.trip_type && t.active == true)
                    .Select(t => t.destination_id)
                    .Distinct()
                    .ToList();

                // 2. Get all destinations (with filters)
                var allDestinations = _db.destinationwithdetails
                    .Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower()
                                 && wr.active == true
                                 && wr.trans_active == true
                                 && wr.country_code.ToLower() == (string.IsNullOrEmpty(req.country_code)
                                        ? wr.country_code.ToLower()
                                        : req.country_code.ToLower()))
                    .Select(dest => new DestinationResponse
                    {
                        destination_id = dest.destination_id,
                        id = (int)dest.id,
                        country_code = dest.country_code,
                        active = dest.active,
                        dest_code = dest.dest_code,
                        dest_description = dest.dest_description,
                        dest_name = dest.dest_name,
                        img_path = dest.img_path,
                        lang_code = dest.lang_code,
                        dest_default_name = dest.dest_default_name,
                        route = dest.route,
                        leaf = dest.leaf,
                        parent_id = dest.parent_id,
                        order = dest.order
                    })
                    .ToList();

                //// 3. Keep only destinations that either have trips or are ancestors of those
                //var filtered = allDestinations
                //    .Where(d =>
                //        tripDestinations.Contains((int)d.destination_id) 


                //    )
                //    .ToList();
                var neededDestinations = new HashSet<int?>();

                foreach (var destId in tripDestinations)
                {
                    var current = allDestinations.FirstOrDefault(d => d.destination_id == destId);
                    while (current != null && !neededDestinations.Contains(current.destination_id))
                    {
                        neededDestinations.Add(current.destination_id);
                        current = allDestinations.FirstOrDefault(d => d.destination_id == current.parent_id);
                    }
                }

                // 4. Filter only the needed destinations
                var filtered = allDestinations
                    .Where(d => neededDestinations.Contains(d.destination_id))
                    .ToList();
                // 4. Build tree recursively
                var tree = GetDestination_TreeMain(filtered, 0); // assuming parent=0 is root
                return tree;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DestinationTree> GetDestination_TreeOld(DestinationReq req)
        {
            try
            {
                var records = from dest in _db.destinationwithdetails.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() &&
                                                                 wr.active == true &&
                                                                 wr.trans_active == true &&
                                                                 wr.country_code.ToLower() == (System.String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower())

                                                          )
                              join trip in _db.trip_mains.Where(wr => wr.trip_type == req.trip_type && wr.active == true) on dest.destination_id equals trip.destination_id into DestAll
                              from combined in DestAll.DefaultIfEmpty()               // LEFT JOIN
                              select new DestinationResponse
                              {
                                  destination_id = dest.destination_id,
                                  id = (int) dest.id!,
                                  country_code = dest.country_code,
                                  active = dest.active,
                                  dest_code = dest.dest_code,
                                  dest_description = dest.dest_description,
                                  dest_name = dest.dest_name,
                                  img_path = dest.img_path,
                                  lang_code = dest.lang_code,
                                  dest_default_name = dest.dest_default_name,
                                  route = dest.route,
                                  leaf = dest.leaf,
                                  parent_id = dest.parent_id,
                                  order = dest.order,
                                  trip_type = combined != null ? combined.trip_type : 0
                              };
                var main = records.ToList().GroupBy(g => new
                {
                    g.route,
                    g.parent_id,
                    g.destination_id,
                    g.img_path,
                    g.active,
                    g.country_code,
                    g.dest_code,
                    g.dest_default_name,
                    g.dest_description,
                    g.dest_name,
                    g.lang_code,
                    g.order,
                    g.leaf,
                    g.id
                }).Select(s => new DestinationResponse
                {
                    destination_id = s.Key.destination_id,
                    id = s.Key.id,
                    country_code = s.Key.country_code,
                    active = s.Key.active,
                    dest_code = s.Key.dest_code,
                    dest_description = s.Key.dest_description,
                    dest_name = s.Key.dest_name,
                    img_path = s.Key.img_path,
                    lang_code = s.Key.lang_code,
                    dest_default_name = s.Key.dest_default_name,
                    route = s.Key.route,
                    leaf = s.Key.leaf,
                    parent_id = s.Key.parent_id,
                    order = s.Key.order,
                }).ToList();
                //var main = _db.destinationwithdetails.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() &&
                //                                                  wr.active == true &&
                //                                                  wr.trans_active == true &&
                //                                                  wr.country_code.ToLower() == (System.String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower()) 

                //                                           )

                //    .Select(s => new DestinationResponse
                //                                           {
                //                                               destination_id = s.destination_id,
                //                                               id = s.id,
                //                                               country_code = s.country_code,
                //                                               active = s.active,
                //                                               dest_code = s.dest_code,
                //                                               dest_description = s.dest_description,
                //                                               dest_name = s.dest_name,
                //                                               img_path = s.img_path,
                //                                               lang_code = s.lang_code,
                //                                               dest_default_name = s.dest_default_name,
                //                                               route = s.route,
                //                                               leaf = s.leaf,
                //                                               parent_id = s.parent_id,
                //                                               order = s.order,
                //                                               trip_type=s.trip_type
                //                                           }).ToList();
                //.Join(_db.trip_mains.Where(wr => wr.trip_type == req.trip_type && wr.active == true),
                //       DEST => new { DEST.destination_id },
                //       TRIP => new { TRIP.destination_id },
                //       (DEST, TRIP) => new DestinationResponse
                //       {
                //           destination_id = DEST.destination_id,
                //           id = DEST.id,
                //           country_code = DEST.country_code,
                //           active = DEST.active,
                //           dest_code = DEST.dest_code,
                //           dest_description = DEST.dest_description,
                //           dest_name = DEST.dest_name,
                //           img_path = DEST.img_path,
                //           lang_code = DEST.lang_code,
                //           dest_default_name = DEST.dest_default_name,
                //           route = DEST.route,
                //           leaf = DEST.leaf,
                //           parent_id = DEST.parent_id,
                //           order = DEST.order
                //       })

                //.GroupBy(g => new
                //       {
                //           g.route,
                //           g.parent_id,
                //           g.destination_id,
                //           g.img_path,
                //           g.active,
                //           g.country_code,
                //           g.dest_code,
                //           g.dest_default_name,
                //           g.dest_description,
                //           g.dest_name,
                //           g.lang_code,
                //           g.order,
                //           g.leaf,
                //           g.id
                //       }).Select(s => new DestinationResponse
                //       {
                //           destination_id = s.Key.destination_id,
                //           id = s.Key.id,
                //           country_code = s.Key.country_code,
                //           active = s.Key.active,
                //           dest_code = s.Key.dest_code,
                //           dest_description = s.Key.dest_description,
                //           dest_name = s.Key.dest_name,
                //           img_path = s.Key.img_path,
                //           lang_code = s.Key.lang_code,
                //           dest_default_name = s.Key.dest_default_name,
                //           route = s.Key.route,
                //           leaf = s.Key.leaf,
                //           parent_id = s.Key.parent_id,
                //           order = s.Key.order,
                //       }).ToList();
                //var main = from trans in _db.destination_translations.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() && wr.active == true)
                //           join dest in _db.destination_mains.Where(wr => wr.active == true && wr.country_code.ToLower() == (System.String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower())) on trans.destination_id equals dest.id         // INNER JOIN
                //           join img in _db.destination_imgs.Where(wr => wr.is_default == true) on trans.id equals img.destination_id into DestAll
                //           from combined in DestAll.DefaultIfEmpty()               // LEFT JOIN
                //           select new DestinationResponse
                //           {
                //               destination_id = trans.destination_id,
                //               id = trans.id,
                //               country_code = dest.country_code,
                //               active = dest.active,
                //               dest_code = dest.dest_code,
                //               dest_description = trans.dest_description,
                //               dest_name = trans.dest_name,
                //               img_path = combined != null ? "http://api.raccoon24.de/" + combined.img_path : null,
                //               lang_code = trans.lang_code,
                //               dest_default_name = dest.dest_default_name,
                //               route = dest.route,
                //               leaf = dest.leaf,
                //               parent_id = dest.parent_id,
                //               order = dest.order,
                //           };


                var result = GetDestination_TreeMain(main.ToList(), 0)

                // .Where(r => r.children != null && r.children.Any() && r.children.Count > 0)
                .ToList();
                //var result = GetDestination_TreeMain(main, 0, req.trip_type)
                //       .Where(r => r != null) // remove null roots
                //       .ToList();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //private bool HasTripType1(DestinationTree node)
        //{
        //    if (node.trip_type == 1)
        //        return true;

        //    return node.children != null && node.children.Any(HasTripType1);
        //}
        public List<DestinationTree> GetDestination_TreeMain(List<DestinationResponse> lst, int? parentId)
        {

            return lst
                   .Where(x => x.parent_id == parentId)
                   .ToList()
                  .Select(s => new DestinationTree
                  {
                      leaf = s.leaf,
                      lang_code = s.lang_code,
                      parent_id = s.parent_id,
                      active = s.active,
                      country_code = s.country_code,
                      destination_id = s.destination_id,
                      dest_code = s.dest_code,
                      dest_default_name = s.dest_default_name,
                      dest_description = s.dest_description,
                      dest_name = s.dest_name,
                      id = s.id,
                      img_path = s.img_path,
                      route = s.route,
                      order = s.order,
                      //parent_order=s.parent_order,
                      //parent_name=s.parent_name,
                      trip_type = s.trip_type,
                      children = GetDestination_TreeMain(lst, s.destination_id).OrderBy(x => x.order).ToList(),

                  })
                  .OrderBy(x => x.order)
                .ToList();
        }
        //        public List<DestinationTree> GetDestination_TreeMain(List<DestinationResponse> lst, int? parentId, int tripType)
        //        {
        //#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        //            return lst
        //                .Where(x => x.parent_id == parentId)
        //                .Select(s =>
        //                {
        //                    // children must match tripType
        //                    var children = GetDestination_TreeMain(lst.Where(c => c.trip_type == tripType).ToList(), s.destination_id, tripType);

        //                    // ✅ keep node if it matches tripType or has matching children
        //                    if (s.trip_type == tripType || children.Any())
        //                    {
        //                        return new DestinationTree
        //                        {
        //                            leaf = s.leaf,
        //                            lang_code = s.lang_code,
        //                            parent_id = s.parent_id,
        //                            active = s.active,
        //                            country_code = s.country_code,
        //                            destination_id = s.destination_id,
        //                            dest_code = s.dest_code,
        //                            dest_default_name = s.dest_default_name,
        //                            dest_description = s.dest_description,
        //                            dest_name = s.dest_name,
        //                            id = s.id,
        //                            img_path = s.img_path,
        //                            route = s.route,
        //                            trip_type = s.trip_type,
        //                            children = children.OrderBy(x => x.order).ToList(),
        //                            order = s.order
        //                        };
        //                    }
        //                    else
        //                    {
        //                        return null;
        //                    }

        //                    //return null;
        //                })
        //                .Where(node => node != null) // remove pruned nodes
        //                .OrderBy(x => x.order)
        //                .ToList();
        //#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        //        }
        #endregion

        #region trips
        //get main categories for trip
        public async Task<List<trip_category>> GetTripCategories()
        {
            return await _db.trip_categories.ToListAsync();
        }
        //get facilities for specific trip
        public List<TripFacility> getFacilityForTrip(long? trip_id, string lang_code, bool? isExtra, bool? is_obligatory)
        {
            try
            {
                var result =
                        from TFAC in _db.trip_facilities
                            .Where(wr => wr.trip_id == trip_id)
                        join TRANS in _db.facility_translations
                            .Where(wr => wr.lang_code.ToLower() == lang_code.ToLower())
                            on TFAC.facility_id equals TRANS.facility_id
                        join FACM in _db.facility_mains
                            .Where(wr => wr.active == true && wr.is_extra ==  (isExtra == false ? wr.is_extra : isExtra) && wr.is_obligatory == (is_obligatory == false ? wr.is_obligatory : is_obligatory))
                            on TFAC.facility_id equals FACM.id
                        select new TripFacility
                        {
                            facility_desc = TRANS.facility_desc,
                            facility_name = TRANS.facility_name,
                            extra_price = FACM.extra_price,
                            currency_code = FACM.currency_code,
                            is_extra = FACM.is_extra,
                            facility_id = TFAC.facility_id,
                            pricing_type = FACM.pricing_type,
                            is_obligatory = FACM.is_obligatory
                        };
                //var result =
                //   from TFAC in _db.trip_facilities.Where(wr => wr.trip_id == trip_id)
                //   join TRANS in _db.facility_translations.Where(wr => wr.lang_code.ToLower() == lang_code.ToLower())
                //   on TFAC.facility_id equals TRANS.facility_id
                //   into TRIPFAC

                //   from combinedFACT in TRIPFAC.DefaultIfEmpty() 
                //   join FACM in _db.facility_mains.Where(wr => wr.active == true && wr.is_extra == isExtra && wr.is_obligatory == is_obligatory)

                //      on TFAC.facility_id equals FACM.id into FacAll
                //   from combinedFACM in FacAll.DefaultIfEmpty() 
                //   select new TripFacility
                //   {
                //       facility_desc = combinedFACT != null ? combinedFACT.facility_desc : "",
                //       facility_name = combinedFACT != null ? combinedFACT.facility_name : "",
                //       extra_price = combinedFACM != null ? combinedFACM.extra_price : 0,
                //       currency_code = combinedFACM != null ? combinedFACM.currency_code : "",
                //       is_extra = combinedFACM != null ? combinedFACM.is_extra : false,
                //       facility_id = TFAC.facility_id,
                //       pricing_type= combinedFACM != null ? combinedFACM.pricing_type : 0,
                //       is_obligatory= combinedFACM != null ? combinedFACM.is_obligatory : null
                //   };

                //var result = from TFAC in _db.trip_facilities.Where(wr => wr.trip_id == trip_id)
                //             join TRANS in _db.facility_translations.Where(wr => wr.lang_code.ToLower() == lang_code.ToLower()) on TFAC.facility_id equals TRANS.facility_id into TRIPFAC
                //             from m in TRIPFAC.DefaultIfEmpty()
                //             select new TripFacility
                //             {
                //                 facility_desc = m.facility_desc,
                //                 facility_name = m.facility_name,


                //             };
                return result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //get images list for specific trip
        public async Task<List<trip_img>> GetImgsByTrip(decimal? trip_id)
        {
            try
            {
                return await _db.trip_imgs.Where(wr => wr.trip_id == trip_id).Select(s => new trip_img
                {
                    id = s.id,
                    img_height = s.img_height,
                    img_name = s.img_name,
                    img_path = "http://api.raccoon24.de/" + s.img_path,
                    img_resize_path = "http://api.raccoon24.de/" + s.img_resize_path,
                    img_width = s.img_width,
                    is_default = s.is_default,
                    trip_id = s.trip_id,
                })
                    //.OrderBy(o => o.img_order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //get trips and top trips with its details 

        public trips_wishlist CheckIfTripInWishList(long? trip_id, string client_id, int? trip_type)
        {
            try
            {
                var result = _db.trips_wishlists.Where(wr => wr.trip_id == trip_id && wr.client_id == client_id && wr.trip_type == trip_type).SingleOrDefault();
                if (result == null)
                {
                    return new trips_wishlist();
                }
                else
                {
                    return result;

                }
            }
            catch (Exception ex)
            {
                return new trips_wishlist();
            }
        }
        public async Task<List<TripsAll>> GetTripsAll(TripsReq req)
        {
            try
            {
                var trips = await _db.tripwithdetails
                    .Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() &&
                                 wr.trip_type == (req.trip_type == 0 ? wr.trip_type : req.trip_type) &&
                                 wr.show_in_top == (req.show_in_top == false ? wr.show_in_top : req.show_in_top) &&
                                 wr.destination_id == (req.destination_id == 0 ? wr.destination_id : req.destination_id) &&
                                 //wr.currency_code.ToLower() == req.currency_code.ToLower() &&
                                 //(string.IsNullOrEmpty(wr.currency_code) || wr.currency_code.ToLower() == req.currency_code.ToLower()) &&
                                 //(string.IsNullOrEmpty(wr.transfer_currency) || wr.transfer_currency.ToLower() == req.currency_code.ToLower()) &&
                                 wr.show_in_slider == (req.show_in_slider == false ? wr.show_in_slider : req.show_in_slider))
                    .OrderBy(o => o.trip_order)
                    .ToListAsync();

                return trips.Select(s => MapToTripsAll(s, req.currency_code, req.client_id)).ToList();
                //return trips.Select(s => new TripsAll
                //{
                //    destination_id = s.destination_id,
                //    lang_code = s.lang_code,
                //    country_code = s.country_code,
                //    currency_code = req.currency_code,
                //    default_img = "http://api.raccoon24.de/" + s.default_img,
                //    dest_code = s.dest_code,
                //    dest_default_name = s.dest_default_name,
                //    pickup = s.pickup,
                //    show_in_slider = s.show_in_slider,
                //    show_in_top = s.show_in_top,
                //    trip_code = s.trip_code,
                //    trip_default_name = s.trip_default_name,
                //    trip_description = s.trip_description,
                //    trip_duration = s.trip_duration,
                //    trip_highlight = s.trip_highlight,
                //    trip_id = s.trip_id,
                //    trip_includes = s.trip_includes,
                //    trip_name = s.trip_name,
                //    //trip_origin_price = s.trip_origin_price,
                //    //trip_sale_price = s.trip_sale_price,
                //    trip_trans_id = s.trip_trans_id,
                //    isfavourite = string.IsNullOrEmpty(req.client_id) ? false : (CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).id == 0 ? false : true),
                //    trip_type = s.trip_type,
                //    wish_id = string.IsNullOrEmpty(req.client_id) ? 0 : CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).id,
                //    wsh_created_at = string.IsNullOrEmpty(req.client_id) ? null : (CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).created_at == null ? null : CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).created_at.Value.ToString("dd-MM-yyyy")),
                //    //wsh_created_at = null,
                //    dest_route = s.dest_route,
                //    route = s.route,
                //    client_id = req.client_id,
                //    facilities = getFacilityForTrip(s.trip_id, s.lang_code,false).ToList(),
                //    imgs = GetImgsByTrip(s.trip_id).Result,
                //    important_info = s.important_info,
                //    trip_details = s.trip_details,
                //    trip_not_includes = s.trip_not_includes,
                //    //max_capacity = s.max_capacity,
                //    //min_capacity = s.min_capacity,
                //    //max_price = s.max_price,
                //    //min_price = s.min_price,
                //    //transfer_category_name = s.transfer_category_name,
                //    //transfer_category__code = s.transfer_category__code,
                //    //transfer_currency = s.transfer_currency,
                //    trip_category_code = s.trip_category_code,
                //    trip_category_name = s.trip_category_name,
                //    //transfer_category_notes = s.transfer_category_notes,
                //    //transfer_child_price = s.transfer_child_price,
                //    //trip_child_price = s.trip_child_price,
                //    //trip_price_notes = s.trip_price_notes,
                //    cancelation_policy=s.cancelation_policy,
                //    release_days=s.release_days,
                //    trip_code_auto =s.trip_code_auto,
                //    trip_max_price = _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Max(m => m.trip_sale_price),
                //    trip_min_price = _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Min(m => m.trip_sale_price),
                //    trip_max_capacity = _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Max(m => m.pax_to),
                //    total_reviews = _db.tbl_reviews.Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type).Count(),
                //    review_rate = _db.tbl_reviews.Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type).Max(m => m.review_rate)
                //}).ToList();

            }
            catch (Exception ex)
            {
                return new List<TripsAll>();
            }
        }

        public async Task<TripsAll> GetTripDetails(TripDetailsReq req)
        {
            try
            {
                var trips = await _db.tripwithdetails
                    .Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() &&
                                 wr.trip_id == req.trip_id &&
                                 wr.trip_type == (req.trip_type == 0 ? wr.trip_type : req.trip_type)
                                 //&& wr.currency_code.ToLower() == req.currency_code.ToLower()
                                 //(string.IsNullOrEmpty(wr.currency_code) || wr.currency_code.ToLower() == req.currency_code.ToLower())
                                 //&& (string.IsNullOrEmpty(wr.transfer_currency) || wr.transfer_currency.ToLower() == req.currency_code.ToLower())

                                 )
                    .ToListAsync();
                var result = trips.Select(s => MapToTripsAll(s, req.currency_code, req.client_id)).ToList();
                //var result = trips.Select(s => new TripsAll
                //{
                //    destination_id = s.destination_id,
                //    lang_code = s.lang_code,
                //    country_code = s.country_code,
                //    currency_code = req.currency_code,
                //    default_img = "http://api.raccoon24.de/" + s.default_img,
                //    dest_code = s.dest_code,
                //    dest_default_name = s.dest_default_name,
                //    pickup = s.pickup,
                //    show_in_slider = s.show_in_slider,
                //    show_in_top = s.show_in_top,
                //    trip_code = s.trip_code,
                //    trip_default_name = s.trip_default_name,
                //    trip_description = s.trip_description,
                //    trip_duration = s.trip_duration,
                //    trip_highlight = s.trip_highlight,
                //    trip_id = s.trip_id,
                //    trip_includes = s.trip_includes,
                //    trip_name = s.trip_name,
                //    //trip_origin_price = s.trip_origin_price,
                //    //trip_sale_price = s.trip_sale_price,
                //    trip_trans_id = s.trip_trans_id,
                //    isfavourite = string.IsNullOrEmpty(req.client_id) ? false : (CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).id == 0 ? false : true),
                //    trip_type = s.trip_type,
                //    wish_id = string.IsNullOrEmpty(req.client_id) ? 0 : CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).id,
                //    wsh_created_at = string.IsNullOrEmpty(req.client_id) ? null : (CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).created_at == null ? null : CheckIfTripInWishList(s.trip_id, req.client_id, s.trip_type).created_at.Value.ToString("dd-MM-yyyy")),
                //    //wsh_created_at = null,
                //    dest_route = s.dest_route,
                //    route = s.route,
                //    client_id = req.client_id,
                //    facilities = getFacilityForTrip(s.trip_id, s.lang_code, false).ToList(),
                //    imgs = GetImgsByTrip(s.trip_id).Result,
                //    important_info = s.important_info,
                //    trip_details = s.trip_details,
                //    trip_not_includes = s.trip_not_includes,
                //    //trip_price_notes = s.trip_price_notes,
                //    //trip_child_price = s.trip_child_price,
                //    //transfer_child_price = s.transfer_child_price,
                //    //transfer_category_notes = s.transfer_category_notes,
                //    //transfer_currency = s.transfer_currency,
                //    trip_category_name = s.trip_category_name,
                //    trip_category_code = s.trip_category_code,
                //    //max_capacity = s.max_capacity,
                //    //max_price = s.max_price,
                //    //min_capacity = s.min_capacity,
                //    //min_price = s.min_price,
                //    trip_code_auto = s.trip_code_auto,
                //    //transfer_category_name = s.transfer_category_name,
                //    //transfer_category__code = s.transfer_category__code,
                //    cancelation_policy = s.cancelation_policy,
                //    release_days=s.release_days,
                //    trip_max_price = _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Max(m => m.trip_sale_price),
                //    trip_min_price= _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Min(m => m.trip_sale_price),
                //    trip_max_capacity = _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Max(m => m.pax_to),
                //    total_reviews = _db.tbl_reviews.Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type).Count(),
                //    review_rate = _db.tbl_reviews.Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type).Max(m => m.review_rate)
                //}).ToList();

                if (result != null)
                {
                    return result.SingleOrDefault();
                }
                return new TripsAll();
            }
            catch (Exception ex)
            {
                return new TripsAll();
            }
        }
        //get wish list for Specific client
        public async Task<List<TripsAll>> GetClientWishList(ClientWishListReq req)
        {
            try
            {
                var trips = await _db.tripwithdetails
                     .Where(wr => wr.lang_code == req.lang_code &&
                                   // wr.currency_code.ToLower() == req.currency_code.ToLower() && 
                                   //(string.IsNullOrEmpty(wr.currency_code) || wr.currency_code.ToLower() == req.currency_code.ToLower()) &&
                                   //&& (string.IsNullOrEmpty(wr.transfer_currency) || wr.transfer_currency.ToLower() == req.currency_code.ToLower()) &&
                                   wr.trip_type == (req.trip_type == 0 ? wr.trip_type : req.trip_type))
                     .Join(_db.trips_wishlists.Where(wr => wr.client_id == req.client_id),
                                    TRIP => new { TRIP.trip_id },
                                    WSH => new { WSH.trip_id },
                                    (TRIP, WSH) => new TripsAll
                                    {
                                        destination_id = TRIP.destination_id,
                                        lang_code = TRIP.lang_code,
                                        country_code = TRIP.country_code,
                                        default_img = "http://api.raccoon24.de/" + TRIP.default_img,
                                        dest_code = TRIP.dest_code,
                                        dest_default_name = TRIP.dest_default_name,
                                        pickup = TRIP.pickup,
                                        show_in_slider = TRIP.show_in_slider,
                                        show_in_top = TRIP.show_in_top,
                                        trip_code = TRIP.trip_code,
                                        trip_default_name = TRIP.trip_default_name,
                                        trip_description = TRIP.trip_description,
                                        trip_duration = TRIP.trip_duration,
                                        trip_highlight = TRIP.trip_highlight,
                                        trip_id = TRIP.trip_id,
                                        trip_includes = TRIP.trip_includes,
                                        trip_name = TRIP.trip_name,
                                        trip_trans_id = TRIP.trip_trans_id,
                                        wish_id = WSH.id,
                                        client_id = WSH.client_id,
                                        wsh_created_at = (WSH != null && WSH.created_at != null) ? WSH.created_at.Value.ToString("dd-MM-yyyy") : null,
                                        trip_type = TRIP.trip_type,
                                        isfavourite = (WSH != null && WSH.id != 0) ? true : false,
                                        dest_route = TRIP.dest_route,
                                        important_info = TRIP.important_info,
                                        route = TRIP.route,
                                        trip_not_includes = TRIP.trip_not_includes,
                                        trip_details = TRIP.trip_details,
                                        trip_category_code = TRIP.trip_category_code,
                                        trip_category_name = TRIP.trip_category_name,
                                        cancelation_policy = TRIP.cancelation_policy,
                                        currency_code = req.currency_code,
                                        trip_code_auto = TRIP.trip_code_auto,
                                        release_days = TRIP.release_days,
                                        is_comm_soon = TRIP.is_comm_soon
                                    }).ToListAsync();

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
                    trip_trans_id = s.trip_trans_id,
                    wish_id = s.wish_id,
                    client_id = s.client_id,
                    wsh_created_at = s.wsh_created_at,
                    trip_type = s.trip_type,
                    isfavourite = s.isfavourite,
                    dest_route = s.dest_route,
                    important_info = s.important_info,
                    route = s.route,
                    trip_not_includes = s.trip_not_includes,
                    trip_details = s.trip_details,
                    total_reviews = _db.tbl_reviews.Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type).Count(),
                    review_rate = _db.tbl_reviews.Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type).Max(m => m.review_rate),
                    facilities = getFacilityForTrip(s.trip_id, s.lang_code, false, false).ToList(),
                    imgs = GetImgsByTrip(s.trip_id).Result,
                    trip_category_code = s.trip_category_code,
                    trip_category_name = s.trip_category_name,
                    trip_code_auto = s.trip_code_auto,
                    cancelation_policy = s.cancelation_policy,
                    release_days = s.release_days,
                    trip_max_price = _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Max(m => m.trip_sale_price),
                    trip_min_price = _db.trip_prices.Where(wr => wr.trip_id == s.trip_id && wr.currency_code == req.currency_code).Min(m => m.trip_sale_price),
                    pricing_type = (_db.trip_prices
                            .Where(wr => wr.trip_id == s.trip_id
                                      && wr.currency_code.ToLower() == req.currency_code.ToLower())
                            .OrderBy(o => o.trip_sale_price)
                            .FirstOrDefault()?.pricing_type) == 1 ? "Per Pax" : "Per Unit",
                    is_comm_soon = s.is_comm_soon,

                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private TripsAll MapToTripsAll(tripwithdetail s, string? currency_code, string? client_id)
        {

            var wishlist = (string.IsNullOrEmpty(client_id)
            ? null
            : CheckIfTripInWishList(s.trip_id, client_id, s.trip_type));

            decimal? trip_max_price = _db.trip_prices
                    .Where(wr => wr.trip_id == s.trip_id && wr.currency_code.ToLower() == currency_code.ToLower())
                    .Max(m => m.trip_sale_price);

            var tripMinRows = _db.trip_prices
                            .Where(wr => wr.trip_id == s.trip_id
                                      && wr.currency_code.ToLower() == currency_code.ToLower())
                            .OrderBy(o => o.trip_sale_price)
                            .FirstOrDefault();
            return new TripsAll
            {
                destination_id = s.destination_id,
                lang_code = s.lang_code,
                country_code = s.country_code,
                currency_code = currency_code,
                default_img = "http://api.raccoon24.de/" + s.default_img,
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
                trip_trans_id = s.trip_trans_id,
                isfavourite = (wishlist != null && wishlist.id != 0),
                trip_type = s.trip_type,
                wish_id = wishlist?.id ?? 0,
                wsh_created_at = wishlist?.created_at?.ToString("dd-MM-yyyy"),
                dest_route = s.dest_route,
                route = s.route,
                client_id = client_id,
                facilities = getFacilityForTrip(s.trip_id, s.lang_code, false, false).ToList(),
                imgs = GetImgsByTrip(s.trip_id).Result,
                important_info = s.important_info,
                trip_details = s.trip_details,
                trip_not_includes = s.trip_not_includes,
                trip_category_name = s.trip_category_name,
                trip_category_code = s.trip_category_code,
                trip_code_auto = s.trip_code_auto,
                cancelation_policy = s.cancelation_policy,
                release_days = s.release_days,
                trip_max_price = trip_max_price,
                trip_min_price = tripMinRows?.trip_sale_price,
                pricing_type = tripMinRows?.pricing_type == 1 ? "Per Pax" : "Per Unit",
                pricing_type_id = tripMinRows?.pricing_type,
                //trip_min_price = _db.trip_prices
                //    .Where(wr => wr.trip_id == s.trip_id && wr.currency_code.ToLower() == currency_code.ToLower())
                //    .Min(m => m.trip_sale_price),
                trip_max_capacity = _db.trip_prices
                    .Where(wr => wr.trip_id == s.trip_id && wr.currency_code.ToLower() == currency_code.ToLower())
                    .Max(m => m.pax_to),
                total_reviews = _db.tbl_reviews
                    .Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type)
                    .Count(),
                review_rate = _db.tbl_reviews
                    .Where(wr => wr.trip_id == s.trip_id && wr.trip_type == s.trip_type)
                    .Max(m => m.review_rate),
                max_child_age = _db.child_policy_settings
                    .Where(wr => wr.trip_id == s.trip_id && wr.currency_code.ToLower() == currency_code.ToLower())
                    .Max(m => m.age_to),
                trip_order = s.trip_order,
                is_comm_soon = s.is_comm_soon,
                dest_name=s.dest_name
                // child_lst = GetChild_Prices(currency_code ,s.trip_id, trip_max_price)
            };
        }

        public List<Child_Prices> GetChild_Prices(string? currency_code, long? trip_id, decimal? adult_price)
        {
            /// <summary>
            /// 1 =Free
            /// 2=% of Adult Price
            /// 3=Fixed Amount
            /// </summary>
            try
            {
                var recorded = _db.child_policy_settings
                    .Where(wr => wr.trip_id == trip_id && wr.currency_code!.ToLower() == currency_code!.ToLower() && wr.pricing_type != 1).ToList();
                return recorded.Select(s => new Child_Prices
                {
                    age_from = s.age_from,
                    age_to = s.age_to,
                    child_price = s.pricing_type == 3 ? s.child_price : (adult_price * s.child_price)
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<Child_Prices>();
            }
        }
        //get trips which shown in home page slider
        public async Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req)
        {
            try
            {
                var trips = await _db.tripwithdetails
                    .Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() &&
                                wr.show_in_slider == true &&
                                wr.trip_type == (req.trip_type == 0 ? wr.trip_type : req.trip_type) &&
                                wr.destination_id == (req.destination_id == 0 ? wr.destination_id : req.destination_id)
                                //wr.currency_code.ToLower() == req.currency_code.ToLower()
                                //(string.IsNullOrEmpty(wr.currency_code) || wr.currency_code.ToLower() == req.currency_code.ToLower())
                                //&& (string.IsNullOrEmpty(wr.transfer_currency) || wr.transfer_currency.ToLower() == req.currency_code.ToLower())

                                )
                    .OrderBy(o => o.trip_order)
                    .ToListAsync();


                return trips;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //get Pickups for spicifics trips
        public async Task<List<TripsPickupResponse>> GetPickupsForTrip(PickupsReq req)
        {
            try
            {
                var result = await _db.trip_pickups_mains.Where(wr => wr.trip_id == req.trip_id && wr.trip_type == req.trip_type)
                                   .Join(_db.trip_pickups_translations.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower()),
                                        MAIN => new { trip_pickup_id = (long?) MAIN.id },
                                        TRANS => new { TRANS.trip_pickup_id },
                                        (MAIN, TRANS) => new TripsPickupResponse
                                        {
                                            trip_pickup_id = MAIN.id,
                                            lang_code = TRANS.lang_code,
                                            order = MAIN.order,
                                            pickup_code = MAIN.pickup_code,
                                            pickup_default_name = MAIN.pickup_default_name,
                                            pickup_description = TRANS.pickup_description,
                                            pickup_name = TRANS.pickup_name,
                                            trip_id = MAIN.trip_id,
                                            trip_type = MAIN.trip_type,
                                            duration = MAIN.duration
                                        }
                                       ).OrderBy(x => x.order).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //get clients reviews for specific trip 
        //used for exercusion trip && transfer trip
        //trip_type = 1 mean exercusion 
        //trip_type = 2 mean transfer
        // pageNumber = 1; // Current page number (1-based)
        // pageSize = 10;  // Number of items per page
        public async Task<ClientsReviewsResponse> GetClientsReviews(ClientsReviewsReq req)
        {

            try
            {
                var totalRecords = await _db.tbl_reviews.Where(wr => wr.trip_id == req.trip_id && wr.trip_type == req.trip_type).CountAsync();
                var average_review_rate = await _db.tbl_reviews.Where(wr => wr.trip_id == req.trip_id && wr.trip_type == req.trip_type).MaxAsync(m => m.review_rate);
                var reviews = await _db.tbl_reviews.Where(wr => wr.trip_id == req.trip_id && wr.trip_type == req.trip_type)
                                            .Select(s => new ClientsReviews
                                            {
                                                trip_id = s.trip_id,
                                                client_id = s.client_id,
                                                entry_date = s.entry_date,
                                                entry_dateStr = s.entry_date.ToString(),
                                                id = s.id,
                                                review_description = s.review_description,
                                                review_rate = s.review_rate,
                                                review_title = s.review_title,
                                                trip_type = s.trip_type,
                                            })
                                             .Skip((req.pageNumber - 1) * req.pageSize)
                                             .Take(req.pageSize)
                                            .ToListAsync();

                return new ClientsReviewsResponse
                {
                    reviews = reviews,
                    average_review_rate = average_review_rate,
                    totalPages = totalRecords
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //save client reviews for trip
        public ResponseCls SaveReviewForTrip(tbl_review row)
        {
            ResponseCls response;
            long maxId = 0;
            try
            {
                row.entry_date = DateTime.Now;
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.tbl_reviews.Where(wr => wr.client_id == row.client_id && wr.trip_id == row.trip_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["AddReviewDuplicate"] };
                    }
                    if (_db.tbl_reviews.Count() > 0)
                    {
                        maxId = _db.tbl_reviews.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.tbl_reviews.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    _db.tbl_reviews.Update(row);
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

        //save client wishList

        public ResponseCls AddTripToWishList(TripsWishlistReq cls)
        {
            ResponseCls response;
            long maxId = 0;
            try
            {
                trips_wishlist row = new trips_wishlist
                {
                    client_id = cls.client_id,
                    created_at = DateTime.Now,
                    id = cls.id,
                    trip_id = cls.trip_id,
                    trip_type = cls.trip_type
                };
                if (cls.delete == true)
                {
                    _db.Remove(row);
                    _db.SaveChanges();
                    return new ResponseCls { errors = null, success = true };
                }
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.trips_wishlists.Where(wr => wr.client_id == row.client_id && wr.trip_id == row.trip_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.trips_wishlists.Count() > 0)
                    {
                        maxId = _db.trips_wishlists.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.trips_wishlists.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    _db.trips_wishlists.Update(row);
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

        //get count of wishlist for specific client

        public async Task<int> GetWishListCount(string client_id)
        {
            try
            {
                return await _db.trips_wishlists.Where(wr => wr.client_id == client_id).CountAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region "booking"

        public async Task<BookingSummary> GetBookingWithDetails(BookingReq req)
        {
            try
            {
                var result = await _db.bookingwithdetails.Where(
                                                        wr => wr.lang_code == req.lang_code &&
                                                              wr.booking_id == req.booking_id &&
                                                              wr.client_id == req.client_id
                                                         ).SingleOrDefaultAsync();
                if (result != null)
                {
                    return new BookingSummary
                    {
                        client_name = result.client_name,
                        trip_type = result.trip_type,
                        trip_id = result.trip_id,
                        booking_code = result.booking_code,
                        booking_code_auto = result.booking_code_auto,
                        booking_date = result.booking_date,
                        booking_datestr = result.booking_datestr,
                        booking_id = result.booking_id,
                        booking_notes = result.booking_notes,
                        booking_status = result.booking_status,
                        booking_status_id = result.booking_status_id,
                        cancelation_policy = result.cancelation_policy,
                        child_num = result.child_num,
                        client_email = result.client_email,
                        client_id = result.client_id,
                        client_nationality = result.client_nationality,
                        client_phone = result.client_phone,
                        currency_code = result.currency_code,
                        default_img = result.default_img,
                        gift_code = result.gift_code,
                        infant_num = result.infant_num,
                        lang_code = result.lang_code,
                        pickup_address = result.pickup_address,
                        pickup_time = result.pickup_time,
                        review_rate = result.review_rate,
                        total_pax = result.total_pax,
                        total_price = result.total_price,
                        trip_code = result.trip_code,
                        trip_date = result.trip_date,
                        trip_datestr = result.trip_datestr,
                        trip_description = result.trip_description,
                        trip_name = result.trip_name,
                        release_days = result.release_days,
                        trip_code_auto = result.trip_code_auto,
                        is_two_way = result.is_two_way,
                        trip_return_date = result.trip_return_date,
                        trip_return_datestr = result.trip_return_datestr,
                        child_ages = result.child_ages,
                        pricing_type = result.pricing_type,
                        extras = GetExtraAssignedToBooking(result.booking_id, req.lang_code, false).ToList(),
                        extras_obligatory = GetExtraAssignedToBooking(result.booking_id, req.lang_code, true).ToList()
                    };
                }
                return new BookingSummary();
            }
            catch (Exception ex)
            {
                return new BookingSummary();
            }
        }

        public async Task<ResponseCls> CancelBooking(long? booking_id,string? client_id)
        {
            ResponseCls response = new ();
            try
            {
                var booking = await _db.trips_bookings.Where(wr => wr.id == booking_id && wr.client_id == client_id).SingleOrDefaultAsync();
                //var booking = await _db.trips_bookings.FirstOrDefaultAsync(wr => wr.id == booking_id && wr.client_id == client_id);
                if (booking != null)
                {
                    booking.booking_status = 3;
                    _db.trips_bookings.Update(booking);
                    await _db.SaveChangesAsync();
                    response = new ResponseCls { errors = null, msg = "", success = true };
                }
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = null, msg = "", success = true };
            }
            return response;
        }

        public async Task<BookingWithTripDetailsAll> ConfirmBooking(ConfirmBookingReq req)
        {

            try
            {
                trips_booking booking = await _db.trips_bookings.Where(wr => wr.id == req.booking_id).SingleOrDefaultAsync();
                if (booking != null)
                {
                    booking.booking_status = 2;
                    _db.trips_bookings.Update(booking);
                    _db.SaveChanges();
                    var result = await _db.bookingwithdetails.Where(
                                                       wr => wr.lang_code == req.lang_code &&
                                                             wr.booking_id == req.booking_id &&
                                                             wr.client_id == req.client_id
                                                        ).SingleOrDefaultAsync();
                    return new BookingWithTripDetailsAll
                    {
                        booking_code = result.booking_code,
                        booking_datestr = result.booking_datestr,
                        booking_id = result.booking_id,
                        child_num = result.child_num,
                        client_email = result.client_email,
                        client_id = result.client_id,
                        client_nationality = result.client_nationality,
                        client_phone = result.client_phone,
                        currency_code = result.currency_code,
                        gift_code = result.gift_code,
                        infant_num = result.infant_num,
                        lang_code = result.lang_code,
                        pickup_address = result.pickup_address,
                        pickup_time = result.pickup_time,
                        review_rate = result.review_rate,
                        total_pax = result.total_pax,
                        total_price = result.total_price,
                        trip_code = result.trip_code,
                        trip_datestr = result.trip_datestr,
                        trip_name = result.trip_name,
                        client_name = result.client_name,
                        trip_id = result.trip_id,
                        trip_type = result.trip_type,
                        is_two_way = result.is_two_way,
                        trip_return_datestr = result.trip_return_datestr,
                        extras = GetExtraAssignedToBooking(result.booking_id, req.lang_code, false),
                        extras_obligatory = GetExtraAssignedToBooking(result.booking_id, req.lang_code, true),
                        pickups = GetPickupsForTrip(new PickupsReq { lang_code = req.lang_code, trip_id = result.trip_id, trip_type = result.trip_type }).Result


                    };
                    //send mail to client && itravel reservation team

                }
                return new BookingWithTripDetailsAll();
            }
            catch (Exception ex)
            {
                return new BookingWithTripDetailsAll();
            }

        }
        //public ResponseCls CalculateBookingPrice(long? booking_id , long? trip_id , int? adult_num, int? child_num,string currency,decimal extras_price)

        public BookingPrice CalculateBookingPrice(CalculateBookingPriceReq req)
        {
            BookingPrice response = new BookingPrice();
            decimal? total_price = 0;
            decimal? total_adult_price = 0;
            decimal? total_child_price = 0;
            decimal? final_price = 0;
            decimal? extras_price = 0;
            decimal? obligatory_price = 0;
            //calc price for Optional list of Extras
            if (req.extra_lst != null && req.extra_lst.Count > 0)
            {
                foreach (var item in req.extra_lst)
                {
                    extras_price = extras_price + (item.extra_price * item.extra_count);
                }
            }
            //calc price for Obligatory list of Extras
            if (req.extra_obligatory != null && req.extra_obligatory.Count > 0)
            {
                foreach (var row in req.extra_obligatory)
                {
                    switch (row.pricing_type)
                    {
                        case 1:
                            //per pax
                            obligatory_price = obligatory_price + (row.extra_price * req.adult_num);
                            break;
                        case 2:
                            //per unit
                            obligatory_price = obligatory_price + (row.extra_price * row.extra_count);
                            break;
                    }
                }
            }
            try
            {
                //get trip details
                var trip = _db.trip_mains.Where(wr => wr.id == req.trip_id).SingleOrDefault();
                if (trip != null)
                {
                    var capacity = req.adult_num + req.child_num;
                    //mean trip is diving or excursion or transfer get price data from tbl trip_prices depend on pax capacity
                    //if trip price doesnot contain pax range so skip check againt capacity
                    var price = _db.trip_prices.Where(wr => wr.trip_id == trip.id && wr.currency_code.ToLower() == req.currency_code.ToLower() && wr.pax_from <= capacity && (wr.pax_to >= capacity || wr.pax_to == 0)).SingleOrDefault();
                    //if(trip.trip_type == 2)
                    // {
                    //     //mean transfer price per unit & skip child calc
                    //     total_adult_price = (price?.trip_sale_price);
                    // }
                    if (price?.pricing_type == 2)
                    {
                        //mean price per unit & skip child calc
                        total_adult_price = (price?.trip_sale_price);
                    }
                    else
                    {
                        //mean price per pax & child calc
                        total_adult_price = (price?.trip_sale_price * req.adult_num);
                        //calculte child price depend on policy assigned to trip
                        foreach (var age in req.childAges)
                        {
                            var policy = _db.child_policy_settings.FirstOrDefault(p => age >= p.age_from && age <= p.age_to && p.trip_id == req.trip_id && p.currency_code.ToLower() == req.currency_code.ToLower());
                            if (policy == null) continue;
                            decimal? child_price = 0;
                            switch (policy.pricing_type)
                            {
                                case 1:
                                    child_price = 0;
                                    break;
                                case 2:
                                    child_price = price?.trip_sale_price * (policy.child_price / 100);
                                    break;
                                case 3:
                                    child_price = policy.child_price;
                                    break;

                            }

                            total_child_price += child_price;
                        }
                    }

                    final_price = total_adult_price + total_child_price + extras_price + obligatory_price;
                    //check if this two way or not (in case trip is transfer only) => so final price = finalprice * 2;
                    if (trip.trip_type == 2 && req.is_two_way == true)
                    {
                        final_price = final_price * 2;
                    }
                }
                if (req.booking_id > 0)
                {
                    //update booking
                    var booking = _db.trips_bookings.Where(wr => wr.id == req.booking_id).SingleOrDefault();

                    if (booking != null)
                    {
                        booking.total_price = final_price;
                        _db.trips_bookings.Update(booking);
                        _db.SaveChanges();
                        response = new BookingPrice { success = true, message = _localizer["UpdateBookingPrice"], final_price = final_price, total_adult_price = total_adult_price, total_child_price = total_child_price, opligatory_extras_price = obligatory_price, optional_extras_price = extras_price };
                    }

                }
                response = new BookingPrice { success = true, final_price = final_price, total_adult_price = total_adult_price, total_child_price = total_child_price, optional_extras_price = extras_price, opligatory_extras_price = obligatory_price };
            }
            catch (Exception ex)
            {
                //final_price = 0;
                response = new BookingPrice { message = _localizer["CheckAdmin"], success = false };
            }
            return response;
        }
        public ResponseCls CalculateBookingPriceOld(CalculateBookingPriceReq req)
        {
            decimal? total_price = 0;
            decimal? final_price = 0;
            decimal? extras_price = 0;
            if (req.extra_lst != null && req.extra_lst.Count > 0)
            {
                foreach (var item in req.extra_lst)
                {
                    extras_price = extras_price + (item.extra_price * item.extra_count);
                }
            }
            ResponseCls response = new ResponseCls();
            try
            {
                //get trip details
                var trip = _db.trip_mains.Where(wr => wr.id == req.trip_id).SingleOrDefault();
                if (trip != null)
                {
                    var capacity = req.adult_num + req.child_num;
                    ////mean it trip is transfer type, get price data from tbl transfer_categories 
                    //if (trip.trip_type == 2)
                    //{
                    //    var transfer =  _db.transfer_categories.Where(wr => wr.id == trip.transfer_category_id && wr.min_capacity <= capacity && wr.max_capacity >= capacity && wr.currency_code.ToLower() == req.currency_code.ToLower()).SingleOrDefault();
                    //    total_price = transfer?.max_price ;
                    //}
                    //else
                    //{
                    //mean trip is diving or excursion or transfer get price data from tbl trip_prices depend on pax capacity
                    var price = _db.trip_prices.Where(wr => wr.trip_id == trip.id && wr.currency_code.ToLower() == req.currency_code.ToLower() && wr.pax_from <= capacity && wr.pax_to >= capacity).SingleOrDefault();
                    total_price = (price?.child_price * req.child_num) + (price?.trip_sale_price * req.adult_num);
                    //}
                }
                final_price = total_price + extras_price;
                //update booking
                var booking = _db.trips_bookings.Where(wr => wr.id == req.booking_id).SingleOrDefault();

                if (booking != null)
                {
                    booking.total_price = final_price;
                    _db.trips_bookings.Update(booking);
                    _db.SaveChanges();
                    response = new ResponseCls { errors = null, success = true, idOut = booking.id, msg = _localizer["UpdateBookingPrice"] };
                }
            }
            catch (Exception ex)
            {
                //final_price = 0;
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }
        public ResponseCls SaveClientBooking(BookingCls row)
        {
            long maxId = 0;
            ResponseCls response = new ();
            try
            {
                string bookCode = "BK" + "-" + row.trip_code + "-" + DateTime.Now.ToString("yyyyMMdd");
                trips_booking booking = new trips_booking
                {
                    booking_code = bookCode,
                    booking_date = DateTime.Now,
                    booking_notes = row.booking_notes,
                    booking_status = row.booking_status,
                    child_num = row.child_num,
                    client_email = row.client_email,
                    client_id = row.client_id,
                    id = row.id,
                    pickup_time = row.pickup_time,
                    total_pax = row.total_pax,
                    total_price = row.total_price,
                    trip_code = row.trip_code,
                    trip_id = row.trip_id,
                    trip_date = DateTime.ParseExact(row.trip_dateStr!, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    //booking_code_auto="BK"+
                    client_nationality = row.client_nationality,
                    client_phone = row.client_phone,
                    gift_code = row.gift_code,
                    infant_num = row.infant_num,
                    pickup_address = row.pickup_address,
                    currency_code = row.currency_code,
                    trip_type = row.trip_type,
                    client_name = row.client_name,
                    booking_code_auto = row.booking_code_auto,
                    is_two_way = row.is_two_way,
                    child_ages = row.childAgesArr != null ? string.Join(",", row.childAgesArr) : row.child_ages,
                    pricing_type = row.pricing_type,
                    trip_return_date = row.trip_return_dateStr != null ? DateTime.ParseExact(row.trip_return_dateStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : null
                };
                // booking.total_price = CalculateBookingPrice(booking.trip_id, booking.total_pax, booking.child_num, row.currency_code);
                if (row.id == 0)
                {
                    //save new booking
                    //check duplicate validation
                    //var result = _db.trips_bookings.Where(wr => wr.trip_id == booking.trip_id && wr.booking_status == booking.booking_status && wr.client_id == booking.client_id && wr.booking_date == booking.booking_date).SingleOrDefault();
                    //if (result != null)
                    //{
                    //    return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    //}
                    if (_db.trips_bookings.Count() > 0)
                    {
                        maxId = _db.trips_bookings.Max(d => d.id);

                    }
                    booking.id = maxId + 1;
                    booking.booking_code_auto = "BK_" + booking.id.ToString();
                    _db.trips_bookings.Add(booking);
                    _db.SaveChanges();
                    //save Obligatory Extra List automatic after save Booking for first time
                    row.id = booking.id;
                    var result = CheckObligatoryExtraAndCalcPrice(row);
                    response = new ResponseCls { errors = result.errors, success = result.success, idOut = booking.id, msg = result.success == false ? result.msg : _localizer["BookingMsg"] };
                }
                else
                {
                    _db.trips_bookings.Update(booking);
                    _db.SaveChanges();
                    response = new ResponseCls { errors = null, success = true, idOut = booking.id, msg = _localizer["BookingMsg"] };
                }

            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        public ResponseCls CheckObligatoryExtraAndCalcPrice(BookingCls row)
        {
            ResponseCls response = new ResponseCls();
            try
            {
                
                CalculateBookingPriceReq req = new CalculateBookingPriceReq
                {
                    adult_num = row.total_pax,
                    booking_id = row.id,
                    childAges = row.childAgesArr,
                    child_num = row.child_num,
                    currency_code = row.currency_code,
                    is_two_way = row.is_two_way,
                    trip_id = row.trip_id,
                };
                //get list of extra assigned to trip with obligatory check
                List<TripFacility> extras = getFacilityForTrip(row.trip_id, "en", true, true).ToList();
                List<booking_extra> lst = new List<booking_extra>();
                if (extras != null && extras.Count > 0)
                {

                    lst = extras.Select(s => new booking_extra
                    {
                        booking_id = row.id,
                        id = 0,
                        extra_count = 1,
                        extra_id = (int?)s.facility_id,
                    }).ToList();

                    response = AssignExtraToBooking(lst);
                    if (response.success)
                    {
                        req.extra_obligatory = extras.Select(s => new ExtraWithPrice
                        {
                            extra_price = s.extra_price,
                            extra_count = 1,
                            pricing_type = s.pricing_type
                        }).ToList();
                    }
                    else
                    {
                        new ResponseCls { msg = response.msg, success = (bool)response.success };
                    }
                }
                //calculate price & update booking
                BookingPrice price = CalculateBookingPrice(req);
                response = new ResponseCls { msg = price.message, success = (bool)price.success };

            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }

        public List<TripFacility> GetTrip_Extra_Mains(TripExtraReq req)
        {
                try
                {
                    var result =
                            from TFAC in _db.trip_facilities
                                .Where(wr => wr.trip_id == req.trip_id)
                            join TRANS in _db.facility_translations
                                .Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower())
                                on TFAC.facility_id equals TRANS.facility_id
                            join FACM in _db.facility_mains
                                .Where(wr => wr.active == true && wr.is_extra == req.isExtra && wr.is_obligatory == req.is_obligatory)
                                on TFAC.facility_id equals FACM.id
                            select new TripFacility
                            {
                                facility_desc = TRANS.facility_desc,
                                facility_name = TRANS.facility_name,
                                extra_price = FACM.extra_price,
                                currency_code = FACM.currency_code,
                                is_extra = FACM.is_extra,
                                facility_id = TFAC.facility_id,
                                pricing_type = FACM.pricing_type,
                                is_obligatory = FACM.is_obligatory
                            };
                   
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        public ResponseCls AssignExtraToBooking(List<booking_extra> lst)
        {
            ResponseCls response;
            int maxId = 0;
            int count = 0;
            try
            {
                foreach (var row in lst)
                {
                    var result = _db.booking_extras.Where(wr => wr.extra_id == row.extra_id && wr.booking_id == row.booking_id).SingleOrDefault();
                    if (result != null)
                    {
                        //update
                        result.updated_at = DateTime.Now;
                        result.extra_count = row.extra_count;
                        _db.booking_extras.Update(result);
                        _db.SaveChanges();
                        //return new ResponseCls { success = false, errors = _localizer["AddExtraDuplicate"] };
                    }
                    else
                    {
                        //save
                        row.created_at = DateTime.Now;
                        if (_db.booking_extras.Count() > 0)
                        {
                            maxId = _db.booking_extras.Max(d => d.id);

                        }
                        row.id = maxId + 1;
                        _db.booking_extras.Add(row);
                        _db.SaveChanges();
                    }
                    //if (row.id == 0)
                    //{
                    //    //check duplicate validation
                    //    var result = _db.booking_extras.Where(wr => wr.extra_id == row.extra_id && wr.booking_id == row.booking_id).SingleOrDefault();
                    //    if (result != null)
                    //    {
                    //        result.extra_count = row.extra_count;
                    //        _db.booking_extras.Update(result);
                    //        _db.SaveChanges();
                    //        continue;
                    //        //return new ResponseCls { success = false, errors = _localizer["AddExtraDuplicate"] };
                    //    }
                    //    if (_db.booking_extras.Count() > 0)
                    //    {
                    //        maxId = _db.booking_extras.Max(d => d.id);

                    //    }
                    //    row.id = maxId + 1;
                    //    _db.booking_extras.Add(row);
                    //    _db.SaveChanges();
                    //}
                    //else
                    //{
                    //    _db.booking_extras.Update(row);
                    //    _db.SaveChanges();
                    //}
                    count++;
                }
                if (count == lst.Count)
                {
                    response = new ResponseCls { errors = null, success = true };
                }
                else
                {
                    response = new ResponseCls { errors = _localizer["BookingExtraSaveError"], success = false, idOut = 0 };
                }
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["BookingExtraSaveError"], success = false, idOut = 0 };
            }
            return response;
        }

        public List<BookingExtraCast> GetExtraAssignedToBooking(long? booking_id, string lang_code, bool is_obligatory)
        {
            try
            {

                var result = from BOOK in _db.booking_extras.Where(wr => wr.booking_id == booking_id)
                             join TRANS in _db.facility_translations.Where(wr => wr.lang_code.ToLower() == lang_code.ToLower()) on BOOK.extra_id equals TRANS.facility_id
                             join FAC in _db.facility_mains.Where(wr => wr.is_obligatory == is_obligatory) on TRANS.facility_id equals FAC.id
                             select new BookingExtraCast
                             {
                                 extra_id = BOOK.extra_id,
                                 booking_id = BOOK.booking_id,
                                 extra_count = BOOK.extra_count,
                                 extra_name = TRANS.facility_name,
                                 extra_price = FAC.extra_price,
                                 id = BOOK.id,
                                 is_obligatory = FAC.is_obligatory,
                                 isExtra=FAC.is_extra,
                                 pricing_type = FAC.pricing_type,
                                 //total_extra_price = FAC!.pricing_type == 1 ? (FAC!.extra_price  * BOOK.extra_count)  : FAC!.extra_price  
                             };

                return result!=null ? [.. result] : [];
            }
            catch (Exception ex)
            {
                return [];
            }
        }
        public async Task<List<BookingSummary>> GetMyBooking(LangReq req, string client_id)
        {
            try
            {
                //get all booking except cancel which status =3
                var records = await _db.bookingwithdetails.Where(wr => wr.client_id == client_id && wr.lang_code.ToLower() == req.lang_code.ToLower() && wr.currency_code.ToLower() == req.currency_code.ToLower() && wr.booking_status_id !=3).ToListAsync();

                return records.Select(result => new BookingSummary
                {
                    client_name = result.client_name,
                    trip_type = result.trip_type,
                    trip_id = result.trip_id,
                    booking_code = result.booking_code,
                    booking_code_auto = result.booking_code_auto,
                    booking_date = result.booking_date,
                    booking_datestr = result.booking_datestr,
                    booking_id = result.booking_id,
                    booking_notes = result.booking_notes,
                    booking_status = result.booking_status,
                    booking_status_id = result.booking_status_id,
                    cancelation_policy = result.cancelation_policy,
                    child_num = result.child_num,
                    client_email = result.client_email,
                    client_id = result.client_id,
                    client_nationality = result.client_nationality,
                    client_phone = result.client_phone,
                    currency_code = result.currency_code,
                    default_img = result.default_img,
                    gift_code = result.gift_code,
                    infant_num = result.infant_num,
                    lang_code = result.lang_code,
                    pickup_address = result.pickup_address,
                    pickup_time = result.pickup_time,
                    review_rate = result.review_rate,
                    total_pax = result.total_pax,
                    total_price = result.total_price,
                    trip_code = result.trip_code,
                    trip_date = result.trip_date,
                    trip_datestr = result.trip_datestr,
                    trip_description = result.trip_description,
                    trip_name = result.trip_name,
                    release_days = result.release_days,
                    trip_code_auto = result.trip_code_auto,
                    is_two_way = result.is_two_way,
                    trip_return_date = result.trip_return_date,
                    trip_return_datestr = result.trip_return_datestr,
                    pricing_type = result.pricing_type,
                    child_ages = result.child_ages,
                    extras = GetExtraAssignedToBooking(result.booking_id, req.lang_code, false).ToList(),
                    extras_obligatory = GetExtraAssignedToBooking(result.booking_id, req.lang_code, true).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<BookingSummary>();
            }
        }
        public async Task<int> GetMyBookingCount(string client_id)
        {
            try
            {
                return await _db.trips_bookings.Where(wr => wr.client_id == client_id && wr.booking_status !=3).CountAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region "Profile"
        //get profile for the client
        public async Task<List<ClientProfileCast>> GetClientProfiles(string clientId)
        {
            try
            {
                return await _db.client_Profiles.Where(wr => wr.client_id == clientId).Select(slc => new ClientProfileCast
                {
                    client_birthday = slc.client_birthday,
                    client_birthdayStr = DateTime.Parse(slc.client_birthday.ToString()).ToString("yyyy-MM-dd"),
                    client_email = slc.client_email,
                    client_id = slc.client_id,
                    client_name = slc.client_name,
                    gender = slc.gender,
                    lang = slc.lang,
                    nation = slc.nation,
                    pay_code = slc.pay_code,
                    phone_number = slc.phone_number,
                    profile_id = slc.profile_id,
                    address = slc.address,
                    client_first_name = slc.client_first_name,
                    client_last_name = slc.client_last_name,

                }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ResponseCls SaveMainProfile(ClientProfileCast profile)
        {
            ResponseCls response;
            decimal maxId = 0;
            try
            {

                if (profile.client_birthdayStr != null)
                {
                    profile.client_birthday = DateOnly.Parse(profile.client_birthdayStr, CultureInfo.InvariantCulture);
                }

                if (profile.profile_id == 0)
                {
                    profile.created_at = DateTime.Now;
                    if (_db.client_Profiles.Count() > 0)
                    {
                        //check validate
                        if (_db.client_Profiles.Where(wr => wr.client_id == profile.client_id).Count() == 0)
                        {
                            maxId = _db.client_Profiles.Max(d => d.profile_id);
                        }
                        else
                        {
                            //do no thing duplicate data
                            return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                        }
                    }
                    profile.profile_id = maxId + 1;
                    _db.client_Profiles.Add(profile);
                }
                else
                {
                    profile.updated_at = DateTime.Now;
                    profile.updated_at = DateTime.Now;
                    _db.Update(profile);
                }
                _db.SaveChanges();
                response = new ResponseCls { success = true, errors = null, idOut = profile.profile_id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { success = false, errors = _localizer["CheckAdmin"], idOut = 0 };
            }
            return response;
        }
        public async Task<ResponseCls> SaveProfileImage(client_image image)
        {
            ResponseCls response;
            decimal maxId = 0;
            try
            {
                //check if client saved profile image before or not (save or update)
                var result = _db.client_images.Where(wr => wr.client_id == image.client_id && wr.type == image.type).SingleOrDefault();
                if (result != null)
                {
                    result.img_path = image.img_path;
                    result.img_name = image.img_name;
                    _db.Update(result);
                }
                else
                {
                    if (_db.client_images.Count() > 0)
                    {
                        maxId = await _db.client_images.DefaultIfEmpty().MaxAsync(d => d.id);
                    }
                    image.id = maxId + 1;
                    _db.client_images.Add(image);
                }


                _db.SaveChanges();
                response = new ResponseCls { success = true, errors = null, idOut = image.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { success = false, errors = _localizer["CheckAdmin"], idOut = 0 };
            }
            return response;
        }

        //get profile image for specific client
        public async Task<List<client_image>> GetProfileImage(string clientId)
        {
            try
            {
                return await _db.client_images.Where(wr => wr.client_id == clientId && wr.type == 1).Select(s => new client_image
                {
                    id = s.id,
                    client_id = s.client_id,
                    img_name = s.img_name,
                    type = s.type,
                    img_path = "http://api.raccoon24.de/" + s.img_path
                }).ToListAsync();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //save clients notification setting
        public ResponseCls SaveClientNotificationSetting(client_notification_setting row)
        {
            ResponseCls response;
            long maxId = 0;
            try
            {
                if (row.id == 0)
                {
                    if (_db.client_notification_settings.Count() > 0)
                    {
                        //check validate
                        if (_db.client_notification_settings.Where(wr => wr.client_id == row.client_id).Count() == 0)
                        {
                            maxId = _db.client_notification_settings.Max(d => d.id);
                        }
                        else
                        {
                            //do no thing duplicate data
                            return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                        }
                    }
                    row.id = maxId + 1;
                    _db.client_notification_settings.Add(row);
                }
                else
                {
                    _db.Update(row);
                }
                _db.SaveChanges();
                response = new ResponseCls { success = true, errors = null, idOut = row.id };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { success = false, errors = _localizer["CheckAdmin"], idOut = 0 };
            }
            return response;
        }


        //get notification setting for specific client
        public async Task<List<client_notification_setting>> GetClient_Notification_Settings(string clientId)
        {
            try
            {
                return await _db.client_notification_settings.Where(wr => wr.client_id == clientId).ToListAsync();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region "Contact"

        public ResponseCls SubscribeNewSletter(newsletter_subscriber row)
        {
            ResponseCls response;
            int maxId = 0;
            try
            {
                row.subscribed_at = DateTime.Now;
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.newsletter_subscribers.Where(wr => wr.client_id == row.client_id).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["AddSletterDuplicate"] };
                    }
                    if (_db.newsletter_subscribers.Count() > 0)
                    {
                        maxId = _db.newsletter_subscribers.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.newsletter_subscribers.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    _db.newsletter_subscribers.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id, msg = _localizer["AddSletterMsg"] };
            }
            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }
        #endregion

    }
}
