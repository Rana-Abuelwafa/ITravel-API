using ITravelApp.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
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

        public ResponseCls AddTripToWishList(TripsWishlistReq row)
        {
            return _clientDAO.AddTripToWishList(row);
        }

        public Task<List<ClientProfileCast>> GetClientProfiles(string clientId)
        {
            return _clientDAO.GetClientProfiles(clientId);
        }

        public Task<ClientsReviewsResponse> GetClientsReviews(ClientsReviewsReq req)
        {
            return _clientDAO.GetClientsReviews(req);
        }

        public Task<List<TripsAll>> GetClientWishList(ClientWishListReq req)
        {
            return _clientDAO.GetClientWishList(req);
        }

        public Task<List<client_notification_setting>> GetClient_Notification_Settings(string clientId)
        {
            return _clientDAO.GetClient_Notification_Settings(clientId);
        }

        public List<DestinationResponse> getDestinations(DestinationReq req)
        {
            return _clientDAO.getDestinations(req);
        }

        public Task<List<TripsPickupResponse>> GetPickupsForTrip(PickupsReq req)
        {
            return _clientDAO.GetPickupsForTrip(req);
        }

        public Task<List<client_image>> GetProfileImage(string clientId)
        {
            return _clientDAO.GetProfileImage(clientId);
        }

        public Task<TripsAll> GetTripDetails(TripDetailsReq req)
        {
            return _clientDAO.GetTripDetails(req);
        }

        public Task<List<TripsAll>> GetTripsAll(TripsReq req)
        {
            return _clientDAO.GetTripsAll(req);
        }

        public Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req)
        {
            return _clientDAO.GetTripsForSlider(req);
        }

        public ResponseCls SaveClientBooking(BookingCls row)
        {
            return _clientDAO.SaveClientBooking(row);
        }

        public ResponseCls SaveClientNotificationSetting(client_notification_setting row)
        {
            return _clientDAO.SaveClientNotificationSetting(row);
        }

        public ResponseCls SaveMainProfile(ClientProfileCast profile)
        {
            return _clientDAO.SaveMainProfile(profile);
        }

        public Task<ResponseCls> SaveProfileImage(client_image image)
        {
            return _clientDAO.SaveProfileImage(image);
        }

        public ResponseCls SaveReviewForTrip(tbl_review row)
        {
            return _clientDAO.SaveReviewForTrip(row);
        }
    }
}
