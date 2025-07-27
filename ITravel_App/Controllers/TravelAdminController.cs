using ITravel_App.Services;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITravel_App.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TravelAdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TravelAdminController(IHttpContextAccessor httpContextAccessor, IAdminService adminService)
        {
            _adminService = adminService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region destination
        [HttpPost("SaveMainDestination")]
        public IActionResult SaveMainDestination(destination_main row)
        {

            return Ok(_adminService.SaveMainDestination(row));
        }
        [HttpPost("SaveDestinationTranslations")]
        public IActionResult SaveDestinationTranslations(destination_translation row)
        {

            return Ok(_adminService.SaveDestinationTranslations(row));
        }
        [HttpPost("saveDestinationImage")]
        public IActionResult saveDestinationImage([FromForm] DestinationImgReq req)
        {
            
            var path = Path.Combine("images" + "//destinations//", req.img.FileName);
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

            destination_img image = new destination_img
            {
                destination_id = req.destination_id,
                img_name = req.img.FileName,
                img_path = path,
                is_default = req.is_default,
                id=req.id
            };

            return Ok(_adminService.saveDestinationImage(image));
        }


        #endregion
        #region trips
        [HttpPost("SaveMainTrip")]
        public IActionResult SaveMainTrip(trip_main row)
        {

            return Ok(_adminService.SaveMainTrip(row));
        }

        [HttpPost("SaveTripTranslation")]
        public IActionResult SaveTripTranslation(TripTranslationReq row)
        {

            return Ok(_adminService.SaveTripTranslation(row));
        }
        [HttpPost("SaveTripPrices")]
        public IActionResult SaveTripPrices(TripPricesReq row)
        {

            return Ok(_adminService.SaveTripPrices(row));
        }
        [HttpPost("saveTripImage")]
        public IActionResult saveTripImage([FromForm] TripImgReq req)
        {
            var path = Path.Combine("images" + "//trips//", req.img.FileName);
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
            return Ok(_adminService.saveTripImage(req));
        }

        [HttpPost("SaveMainFacility")]
        public IActionResult SaveMainFacility(facility_main row)
        {

            return Ok(_adminService.SaveMainFacility(row));
        }
        [HttpPost("SaveFacilityTranslation")]
        public IActionResult SaveFacilityTranslation(FacilityTranslationReq row)
        {

            return Ok(_adminService.SaveFacilityTranslation(row));
        }
        [HttpPost("AssignFacilityToTrip")]
        public IActionResult AssignFacilityToTrip(TripFacilityAssignReq row)
        {

            return Ok(_adminService.AssignFacilityToTrip(row));
        }

        [HttpPost("SaveMainTripPickups")]
        public IActionResult SaveMainTripPickups(TripsPickupSaveReq row)
        {

            return Ok(_adminService.SaveMainTripPickups(row));
        }

        [HttpPost("SaveTripPickupsTranslations")]
        public IActionResult SaveTripPickupsTranslations(TripsPickupTranslationSaveReq row)
        {

            return Ok(_adminService.SaveTripPickupsTranslations(row));
        }
       
        #endregion
    }
}
