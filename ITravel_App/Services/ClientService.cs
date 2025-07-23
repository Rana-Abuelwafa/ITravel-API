using ITravelApp.Data;
using ITravelApp.Data.Models.destination;

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
    }
}
