using ITravel_App.Services;
using ITravelApp.Data.Entities;
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
        public BookingController(IHttpContextAccessor httpContextAccessor, IClientService clientService)
        {
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region reviews
        [HttpPost("GetClientsReviews")] 
        public async Task<IActionResult> GetClientsReviews(ClientsReviewsReq req)
        {
            return Ok(await _clientService.GetClientsReviews(req));
        }
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
    }
}
