using ITravel_App.Services;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.Bookings.Admin;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.Transfer;
using ITravelApp.Data.Models.trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ITravel_App.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TravelAdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginUserData _loginUserData;
        public TravelAdminController(IHttpContextAccessor httpContextAccessor, IAdminService adminService)
        {
            _adminService = adminService;
            _httpContextAccessor = httpContextAccessor;
            _loginUserData = Utils.getTokenData(httpContextAccessor);
        }

        #region "Main_setting"

        [HttpPost("Get_Currencies")]
        public async Task<IActionResult> Get_Currencies()
        {
            return Ok(await _adminService.Get_Currencies());
        }
        [HttpPost("Get_Languages")]
        public async Task<IActionResult> Get_Languages()
        {
            return Ok(await _adminService.Get_Languages());
        }
        #endregion
        #region destination

        [HttpPost("GetDestinationMain")]
        public async Task<IActionResult> GetDestination_Mains([FromQuery] bool leaf)
        {

            return Ok(await _adminService.GetDestination_Mains(leaf));
        }
        [HttpPost("GetDestinationWithTranslations")]
        public IActionResult GetDestinationWithTranslations(DestinationReq row)
        {

            return Ok(_adminService.GetDestinationWithTranslations(row));
        }
        [HttpPost("SaveMainDestination")]
        public IActionResult SaveMainDestination(destination_main row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.SaveMainDestination(row));
        }
        [HttpPost("SaveDestinationTranslations")]
        public IActionResult SaveDestinationTranslations(destination_translation row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.SaveDestinationTranslations(row));
        }
        [HttpPost("saveDestinationImage")]
        public IActionResult saveDestinationImageAsync([FromForm] DestinationImgReq req)
        {
            string email = _loginUserData.client_email;
            List<destination_img> lst = new List<destination_img>();
            foreach (var img in req.imgs)
            {

                var path = Path.Combine("images" + "/destinations/", img.FileName);
                //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        img.CopyTo(stream);
                        stream.Close();
                    }

                }
                catch (Exception ex)
                {

                }
                //using var image = await Image.LoadAsync(req.img.OpenReadStream());
                destination_img image = new destination_img
                {
                    destination_id = req.destination_id,
                    img_name = img.FileName,
                    img_path = path,
                    is_default = req.is_default,
                    id = req.id,
                    created_by = email,
                };
                lst.Add(image);
            }


            return Ok(_adminService.saveDestinationImage(lst));
        }

        [HttpPost("GetImgsByDestination")]
        public async Task<IActionResult> GetImgsByDestination([FromQuery] int destination_id)
        {

            return Ok(await _adminService.GetImgsByDestination(destination_id));
        }

        [HttpPost("UpdateDestinationImage")]
        public IActionResult UpdateDestinationImage(DestinationImgUpdateReq row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.UpdateDestinationImage(row));
        }
        #endregion
        #region trips

        [HttpPost("GetTripCategories")]
        public async Task<IActionResult> GetTripCategories()
        {

            return Ok(await _adminService.GetTripCategories());
        }

        [HttpPost("GetTrip_Mains")]
        public async Task<IActionResult> GetTrip_Mains([FromQuery] int destination_id , [FromQuery] int trip_type)
        {

            return Ok(await _adminService.GetTrip_Mains(destination_id, trip_type));
        }

        [HttpPost("SaveMainTrip")]
        public IActionResult SaveMainTrip(trip_main row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;

            return Ok(_adminService.SaveMainTrip(row));
        }

        [HttpPost("SaveTripTranslation")]
        public IActionResult SaveTripTranslation(TripTranslationReq row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.SaveTripTranslation(row));
        }

        [HttpPost("SaveTripPrices")]
        public IActionResult SaveTripPrices(TripPricesReq row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;

            return Ok(_adminService.SaveTripPrices(row));
        }
        [HttpPost("SaveTripImage")]
        public IActionResult SaveTripImage([FromForm] TripImgReq req)
        {
            string email = _loginUserData.client_email;
            List<trip_img> lst = new List<trip_img>();
            foreach (var img in req.imgs)
            {
                trip_img cls = new trip_img();
                var path = Path.Combine("images" + "/trips/", img.FileName);
                //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        img.CopyTo(stream);
                        stream.Close();
                    }

                }
                catch (Exception ex)
                {

                }
                cls.img_name = img.FileName;
                cls.img_path = path;
                cls.created_by = email;
                cls.id = req.id;
                cls.trip_id = req.trip_id;
                cls.is_default = req.is_default;
                lst.Add(cls);
            }
            //var path = Path.Combine("images" + "/trips/", req.img.FileName);
            ////var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
            //try
            //{
            //    using (FileStream stream = new FileStream(path, FileMode.Create))
            //    {
            //        req.img.CopyTo(stream);
            //        stream.Close();
            //    }

            //}
            //catch (Exception ex)
            //{

            //}
            //req.img_name = req.img.FileName;
            //req.img_path = path;
            //req.created_by=email;
            return Ok(_adminService.saveTripImage(lst));
        }

        [HttpPost("SaveMainFacility")]
        public IActionResult SaveMainFacility(facility_main row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.SaveMainFacility(row));
        }
        [HttpPost("SaveFacilityTranslation")]
        public IActionResult SaveFacilityTranslation(FacilityTranslationReq row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.SaveFacilityTranslation(row));
        }
        [HttpPost("AssignFacilityToTrip")]
        public IActionResult AssignFacilityToTrip(TripFacilityAssignReq row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.AssignFacilityToTrip(row));
        }

        [HttpPost("SaveMainTripPickups")]
        public IActionResult SaveMainTripPickups(TripsPickupSaveReq row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.SaveMainTripPickups(row));
        }

        [HttpPost("SaveTripPickupsTranslations")]
        public IActionResult SaveTripPickupsTranslations(TripsPickupTranslationSaveReq row)
        {
            string email = _loginUserData.client_email;
            row.created_by = email;
            return Ok(_adminService.SaveTripPickupsTranslations(row));
        }

        [HttpPost("GetTripTranslationGrp")]
        public async Task<IActionResult> GetTripTranslationGrp([FromQuery] long trip_id)
        {
            return Ok(await _adminService.GetTripTranslationGrp(trip_id));
        }
        [HttpPost("GetTrip_Prices")]
        public async Task<IActionResult> GetTrip_Prices([FromQuery] long trip_id)
        {
            return Ok(await _adminService.GetTrip_Prices(trip_id));
        }
        [HttpPost("GetPickupsAllForTrip")]
        public IActionResult GetPickupsAllForTrip(PickupsReq req)
        {
            return Ok(_adminService.GetPickupsAllForTrip(req));
        }

        [HttpPost("GetImgsByTrip")]
        public async Task<IActionResult> GetImgsByTrip([FromQuery] long trip_id)
        {
            return Ok(await _adminService.GetImgsByTrip(trip_id));
        }
        [HttpPost("UpdateTripImage")]
        public IActionResult UpdateTripImage(TripImgUpdateReq trip)
        {
            string email = _loginUserData.client_email;
            trip.created_by = email;
            return Ok(_adminService.UpdateTripImage(trip));
        }
        [HttpPost("GetFacilityWithTranslation")]
        public IActionResult GetFacilityWithTranslation()
        {
            return Ok(_adminService.GetFacilityWithTranslation());
        }

        [HttpPost("GetFacilityAllWithSelect")]
        public IActionResult GetFacilityAllWithSelect([FromQuery] long trip_id)
        {
            return Ok(_adminService.GetFacilityAllWithSelect(trip_id));
        }

        [HttpPost("GetTrip_ChildPolicy")]
        public async Task<IActionResult> GetTrip_ChildPolicy([FromQuery] long trip_id)
        {
            return Ok(await _adminService.GetTrip_ChildPolicy(trip_id));
        }
        [HttpPost("SaveTripChildPolicy")]
        public IActionResult SaveTripChildPolicy(ChildPolicyPricesReq trip)
        {
            string email = _loginUserData.client_email;
            trip.created_by = email;
            return Ok(_adminService.SaveTripChildPolicy(trip));
        }
        #endregion

        #region transfer
        [HttpPost("GetTransfer_Categories")]
        public async Task<IActionResult> GetTransfer_Categories()
        {

            return Ok(await _adminService.GetTransfer_Categories());
        }

        [HttpPost("SaveTransferCategory")]
        public IActionResult SaveTransferCategory(TransferCategorySaveReq row)
        {
            string? email = _loginUserData.client_email;
            row.created_by = email;

            return Ok(_adminService.SaveTransferCategory(row));
        }
        #endregion


        #region "Booking"
        [HttpPost("GetAllBooking")]
        public async Task<IActionResult> GetAllBooking(BookingAllReq req)
        {
            return Ok(await _adminService.GetAllBooking(req));
        }
        #endregion
    }
}
