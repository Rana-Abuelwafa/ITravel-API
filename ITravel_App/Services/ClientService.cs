using ITravelApp.Data;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.trips;

namespace ITravel_App.Services
{
    public class ClientService : IClientService
    {
        private ClientDAO _clientDAO;

        public ClientService(ClientDAO clientDAO)
        {
            _clientDAO = clientDAO;

        }

        public List<DestinationResponse> getDestinations(DestinationReq req)
        {
            return _clientDAO.getDestinations(req);
        }

        public Task<List<TripsAll>> GetTripsAll(TripsReq req)
        {
            return _clientDAO.GetTripsAll(req);
        }
    }
}
