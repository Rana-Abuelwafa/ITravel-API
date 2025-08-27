using ITravel_App.Services;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
using ITravelApp.Data.Models.trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITravel_App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginUserData _loginUserData;
        public BookingController(IHttpContextAccessor httpContextAccessor, IClientService clientService)
        {
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
            _loginUserData = Utils.getTokenData(httpContextAccessor);
        }

        #region reviews
       
        [HttpPost("SaveReviewForTrip")]
        public IActionResult SaveReviewForTrip(tbl_review row)
        {
            string? clientId = string.Empty;

            if (_httpContextAccessor.HttpContext is not null)
            {
                clientId = _httpContextAccessor.HttpContext.User.FindFirstValue("ClientId");

            }
            row.client_id=clientId;
            return Ok(_clientService.SaveReviewForTrip(row));
        }
        #endregion

        #region "wishlist"

        [HttpPost("AddTripToWishList")]
        public IActionResult AddTripToWishList(TripsWishlistReq row)
        {
            string? clientId = string.Empty;

            if (_httpContextAccessor.HttpContext is not null)
            {
                clientId = _httpContextAccessor.HttpContext.User.FindFirstValue("ClientId");

            }
            row.client_id = clientId;
            return Ok(_clientService.AddTripToWishList(row));
        }


        [HttpPost("GetClientWishList")]
        public async Task<IActionResult> GetClientWishList(ClientWishListReq req)
        {
            string? clientId = string.Empty;

            if (_httpContextAccessor.HttpContext is not null)
            {
                clientId = _httpContextAccessor.HttpContext.User.FindFirstValue("ClientId");

            }
            req.client_id = clientId;
            return Ok(await _clientService.GetClientWishList(req));
        }
        #endregion


        #region "Booking"
        [HttpPost("SaveClientBooking")]
        public IActionResult SaveClientBooking(BookingCls row)
        {

            string? clientId = _loginUserData.client_id;
            string? email = _loginUserData.client_email;
           
            row.client_id = clientId;
            row.client_email = email;
            return Ok(_clientService.SaveClientBooking(row));
        }
        #endregion


        #region "Profile"
        [HttpPost("GetClientProfiles")]
        public async Task<IActionResult> GetClientProfiles()
        {
            string? clientId = _loginUserData.client_id;
            // string? email = _loginUserData.client_email;
            return Ok(await _clientService.GetClientProfiles(clientId));
        }

        [HttpPost("GetProfileImage")]
        public async Task<IActionResult> GetProfileImage()
        {
            string? clientId = _loginUserData.client_id;
            // string? email = _loginUserData.client_email;
            return Ok(await _clientService.GetProfileImage(clientId));
        }
        [HttpPost("saveProfileImage")]
        public async Task<IActionResult> SaveProfileImage(ImgCls cls)
        {
            string? clientId = _loginUserData.client_id;
            var path = Path.Combine("images" + "//", cls.img.FileName);
            //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                cls.img.CopyTo(stream);
                stream.Close();
            }



            client_image image = new client_image
            {
                client_id = clientId,
                img_name = cls.img.FileName,
                img_path = path,
                type = 1  //mean save profile image
            };

            return Ok(await _clientService.SaveProfileImage(image));
        }
        [HttpPost("SaveMainProfile")]
        public IActionResult SaveMainProfile(ClientProfileCast profile)
        {
            string? clientId = _loginUserData.client_id;
            string? email = _loginUserData.client_email;
            string? FullName = _loginUserData.FullName;
            profile.client_id = clientId;
            profile.client_name = FullName;
            profile.client_email = email;
            return Ok(_clientService.SaveMainProfile(profile));
        }
        [HttpPost("GetClient_Notification_Settings")]
        public async Task<IActionResult> GetClient_Notification_Settings()
        {
            string? clientId = _loginUserData.client_id;
            return Ok(await _clientService.GetClient_Notification_Settings(clientId));
        }
        [HttpPost("SaveClientNotificationSetting")]
        public IActionResult SaveClientNotificationSetting(client_notification_setting row)
        {
            string? clientId = _loginUserData.client_id;
            string? email = _loginUserData.client_email;
            string? FullName = _loginUserData.FullName;
            row.client_id = clientId;
            return Ok(_clientService.SaveClientNotificationSetting(row));
        }
        #endregion
    }
}
