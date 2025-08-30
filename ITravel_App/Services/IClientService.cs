using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
using ITravelApp.Data.Models.trips;

namespace ITravel_App.Services
{
    public interface IClientService
    {
        #region destination
        public List<DestinationResponse> getDestinations(DestinationReq req);
        #endregion

        #region trips
        public Task<TripsAll> GetTripDetails(TripDetailsReq req);
        public Task<List<TripsAll>> GetTripsAll(TripsReq req);
        public Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req);
        public Task<List<TripsPickupResponse>> GetPickupsForTrip(PickupsReq req);
        public Task<ClientsReviewsResponse> GetClientsReviews(ClientsReviewsReq req);

        public ResponseCls SaveReviewForTrip(tbl_review row);
        public Task<List<TripsAll>> GetClientWishList(ClientWishListReq req);
        public ResponseCls AddTripToWishList(TripsWishlistReq row);
        #endregion

        #region "booking"
        public ResponseCls SaveClientBooking(BookingCls row);
        #endregion

        #region "Profile"
        public Task<List<ClientProfileCast>> GetClientProfiles(string clientId);
        public ResponseCls SaveMainProfile(ClientProfileCast profile);
        public Task<ResponseCls> SaveProfileImage(client_image image);
        public Task<List<client_image>> GetProfileImage(string clientId);
        public ResponseCls SaveClientNotificationSetting(client_notification_setting row);
        public Task<List<client_notification_setting>> GetClient_Notification_Settings(string clientId);
        #endregion
    }
}
