using ITravel_App.Services;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
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
    }
}
