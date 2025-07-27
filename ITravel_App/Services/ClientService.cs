using ITravelApp.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
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

        public Task<ClientsReviewsResponse> GetClientsReviews(ClientsReviewsReq req)
        {
            return _clientDAO.GetClientsReviews(req);
        }

        public List<DestinationResponse> getDestinations(DestinationReq req)
        {
            return _clientDAO.getDestinations(req);
        }

        public Task<List<TripsPickupResponse>> GetPickupsForTrip(PickupsReq req)
        {
            return _clientDAO.GetPickupsForTrip(req);
        }

        public Task<List<TripsAll>> GetTripsAll(TripsReq req)
        {
            return _clientDAO.GetTripsAll(req);
        }

        public Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req)
        {
            return _clientDAO.GetTripsForSlider(req);
        }

        public ResponseCls SaveReviewForTrip(tbl_review row)
        {
            return _clientDAO.SaveReviewForTrip(row);
        }
    }
}
