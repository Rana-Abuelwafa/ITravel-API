using ITravelApp.Data.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
using ITravelApp.Data.Models.trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

				var result = from trans in _db.destination_translations.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() && wr.active == true)
							 join dest in _db.destination_mains.Where(wr => wr.active == true && wr.country_code.ToLower() == (System.String.IsNullOrEmpty(req.country_code) ? wr.country_code.ToLower() : req.country_code.ToLower())) on trans.destination_id equals dest.id         // INNER JOIN
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
								 img_path = combined != null ? "http://api.raccoon24.de//" + combined.img_path : null,
								 lang_code = trans.lang_code,
								 dest_default_name=dest.dest_default_name,
								 route=dest.route
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

		//get facilities for specific trip
		public List<TripFacility> getFacilityForTrip(decimal? trip_id, string lang_code)
		{
			try
			{
				var result = from TFAC in _db.trip_facilities.Where(wr => wr.trip_id == trip_id)
							 join TRANS in _db.facility_translations.Where(wr => wr.lang_code.ToLower() == lang_code.ToLower()) on TFAC.facility_id equals TRANS.facility_id into TRIPFAC
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
	   
		//get images list for specific trip
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
		//get trips and top trips with its details 
		public async Task<List<TripsAll>> GetTripsAll(TripsReq req)
		{
			try
			{
				var trips = await _db.tripwithdetails
					.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() && 
								 wr.show_in_top == (req.show_in_top == false ? wr.show_in_top : req.show_in_top) && 
								 wr.destination_id == (req.destination_id == 0 ? wr.destination_id : req.destination_id) &&
								 wr.currency_code.ToLower() == req.currency_code.ToLower())
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
						isfavourite = s.isfavourite,
						trip_type = s.trip_type,
						wish_id = s.wish_id,
						wsh_created_at=s.wsh_created_at,
						dest_route = s.dest_route,
						route=s.route,
						client_id=s.client_id,
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
		
		//get trips which shown in home page slider
		public async Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req)
		{
			try
			{
				var trips = await _db.tripwithdetails
					.Where(wr => wr.lang_code.ToLower() == req.lang_code.ToLower() && 
								wr.show_in_slider == true && 
								wr.destination_id == (req.destination_id == 0 ? wr.destination_id : req.destination_id) &&
								wr.currency_code.ToLower() == req.currency_code.ToLower())
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
										MAIN => new { trip_pickup_id = MAIN.id },
										TRANS => new {TRANS.trip_pickup_id },
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
											duration=MAIN.duration
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
				var totalRecords = await _db.tbl_reviews.CountAsync();
				var reviews = await _db.tbl_reviews.Where(wr => wr.trip_id == req.trip_id && wr.trip_type == req.trip_type)
											.Select(s => new ClientsReviews
											{
												trip_id = s.trip_id,
												client_id = s.client_id,
												entry_date= s.entry_date,
												entry_dateStr=s.entry_date.ToString(),
												id = s.id,
												review_description= s.review_description,
												review_rate= s.review_rate,
												review_title= s.review_title,
												trip_type= s.trip_type,
											})
											 .Skip((req.pageNumber - 1) * req.pageSize)
											 .Take(req.pageSize)
											.ToListAsync();

				return new ClientsReviewsResponse
				{
					reviews = reviews,
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
						return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
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
		//save client reviews for trip
		public ResponseCls AddTripToWishList(trips_wishlist row)
		{
			ResponseCls response;
			long maxId = 0;
			try
			{
				row.created_at = DateTime.Now;
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


		//get wish list for Specific client
		public async Task<List<TripsAll>> GetClientWishList(ClientWishListReq req)
		{
			try
			{
				var trips = await _db.tripwithdetails
					 .Where(wr => wr.lang_code == req.lang_code &&
								   wr.currency_code.ToLower() == req.currency_code.ToLower() &&
								   wr.client_id == req.client_id &&
								   wr.trip_type == req.trip_type &&
								   wr.wish_id > 0
						   )
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
					wish_id = s.wish_id,
					client_id = s.client_id,
					wsh_created_at = s.wsh_created_at,
					trip_type = s.trip_type,
					isfavourite= s.isfavourite,
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
		#endregion

		#region "booking"
		  public ResponseCls SaveClientBooking(BookingCls row)
		{
			long maxId = 0;
			ResponseCls response = null;
			try
			{
				string bookCode = "BK"+"-" + row.trip_code + "-" +DateTime.Now.ToString("yyyyMMdd");
				trips_booking booking = new trips_booking
				{
					booking_code = bookCode,
					booking_date= DateTime.Today,
					booking_notes = row.booking_notes,
					booking_status = row.booking_status,
					child_num = row.child_num,
					client_email = row.client_email,
					client_id=row.client_id,
					id = row.id,
					pickup_time = row.pickup_time,
					total_pax = row.total_pax,
					total_price = row.total_price,
					trip_code = row.trip_code,
					trip_id=row.trip_id,
					trip_date=DateTime.Parse(row.trip_dateStr),

				};
				if (row.id == 0)
				{
					//check duplicate validation
					var result = _db.trips_bookings.Where(wr => wr.trip_id == booking.trip_id && wr.booking_status == booking.booking_status && wr.client_id == booking.client_id && wr.booking_date == booking.booking_date).SingleOrDefault();
					if (result != null)
					{
						return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
					}
					if (_db.trips_bookings.Count() > 0)
					{
						maxId = _db.trips_bookings.Max(d => d.id);

					}
					booking.id = maxId + 1;
					_db.trips_bookings.Add(booking);
					_db.SaveChanges();
				}
				else
				{
					_db.trips_bookings.Update(booking);
					_db.SaveChanges();
				}
                response = new ResponseCls { errors = null, success = true, idOut = booking.id ,msg= _localizer["BookingMsg"] };
            }
			catch (Exception ex) {
				response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
			}
			return response;
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
                    profile.updated_at= DateTime.Now;
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
                    img_path = "http://api.raccoon24.de//" + s.img_path
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
    }
}
