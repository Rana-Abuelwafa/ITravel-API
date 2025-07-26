using ITravel_App.Services;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.trips;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITravel_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TravelClientController(IHttpContextAccessor httpContextAccessor, IClientService clientService)
        {
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region destination
        //get destinations
        [HttpPost("getDestinations")]
        public IActionResult getDestinations(DestinationReq req)
        {

            return Ok(_clientService.getDestinations(req));
        }
        #endregion
        #region trips
        [HttpPost("GetTripsAll")]
        public async Task<IActionResult> GetTripsAll(TripsReq req)
        {

            return Ok(await _clientService.GetTripsAll(req));
        }
        [HttpPost("GetTripsForSlider")]
        public async Task<IActionResult> GetTripsForSlider(TripsReq req)
        {

            return Ok(await _clientService.GetTripsForSlider(req));
        }
        #endregion
    }
}
