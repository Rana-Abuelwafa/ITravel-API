using ITravel_App.Services;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
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
        //get specific trip details
        [HttpPost("GetTripDetails")]
        public async Task<IActionResult> GetTripDetails(TripDetailsReq req)
        {

            return Ok(await _clientService.GetTripDetails(req));
        }
        //get trips and top trips with its details 
        [HttpPost("GetTripsAll")]
        public async Task<IActionResult> GetTripsAll(TripsReq req)
        {

            return Ok(await _clientService.GetTripsAll(req));
        }
        //get trips which shown in home page slider
        [HttpPost("GetTripsForSlider")]
        public async Task<IActionResult> GetTripsForSlider(TripsReq req)
        {

            return Ok(await _clientService.GetTripsForSlider(req));
        }

        [HttpPost("GetPickupsForTrip")]
        public async Task<IActionResult> GetPickupsForTrip(PickupsReq req)
        {

            return Ok(await _clientService.GetPickupsForTrip(req));
        }
        [HttpPost("GetClientsReviews")]
        public async Task<IActionResult> GetClientsReviews(ClientsReviewsReq req)
        {
            return Ok(await _clientService.GetClientsReviews(req));
        }

        #endregion


    }
}
