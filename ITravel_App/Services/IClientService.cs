using ITravelApp.Data.Models.destination;

namespace ITravel_App.Services
{
    public interface IClientService
    {
        public List<DestinationResponse> getDestinations(DestinationReq req);
    }
}
