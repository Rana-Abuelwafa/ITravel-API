using ITravel_App.Services;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace ITravel_App.Controllers
{
    //[Authorize(Roles = "Admin")]
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

        #region destination

        [HttpPost("GetDestinationMain")]
        public async Task<IActionResult> GetDestination_Mains()
        {

            return Ok(await _adminService.GetDestination_Mains());
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
            var path = Path.Combine("images" + "/destinations/", req.img.FileName);
            //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    req.img.CopyTo(stream);
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
                img_name = req.img.FileName,
                img_path = path,
                is_default = req.is_default,
                id=req.id,
                created_by = email,
            };

            return Ok(_adminService.saveDestinationImage(image));
        }
        
        [HttpPost("GetImgsByDestination")]
        public async Task<IActionResult> GetImgsByDestination([FromQuery] int destination_id)
        {

            return Ok(await _adminService.GetImgsByDestination(destination_id));
        }


        #endregion
        #region trips

        [HttpPost("GetTrip_Mains")]
        public async Task<IActionResult> GetTrip_Mains([FromQuery] int destination_id)
        {

            return Ok(await _adminService.GetTrip_Mains(destination_id));
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
        [HttpPost("saveTripImage")]
        public IActionResult saveTripImage([FromForm] TripImgReq req)
        {
            string email = _loginUserData.client_email;
            var path = Path.Combine("images" + "/trips/", req.img.FileName);
            //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    req.img.CopyTo(stream);
                    stream.Close();
                }

            }
            catch (Exception ex)
            {

            }
            req.img_name = req.img.FileName;
            req.img_path = path;
            req.created_by=email;
            return Ok(_adminService.saveTripImage(req));
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

      
        #endregion
    }
}
