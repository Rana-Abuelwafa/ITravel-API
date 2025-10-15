using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.Bookings.Client;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
using ITravelApp.Data.Models.trips;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITravel_App.Services
{
    public interface IClientService
    {
        #region destination
        public Task<List<destinationwithdetail>> getDestinations(DestinationReq req);
        public List<DestinationTree> GetDestination_Tree(DestinationReq req);
        #endregion

        #region trips
        public Task<List<trip_category>> GetTripCategories();
        public Task<TripsAll> GetTripDetails(TripDetailsReq req);
        public Task<List<TripsAll>> GetTripsAll(TripsReq req);
        public Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req);
        public Task<List<TripsPickupResponse>> GetPickupsForTrip(PickupsReq req);
        public Task<ClientsReviewsResponse> GetClientsReviews(ClientsReviewsReq req);

        public ResponseCls SaveReviewForTrip(tbl_review row);
        public Task<List<TripsAll>> GetClientWishList(ClientWishListReq req);
        public ResponseCls AddTripToWishList(TripsWishlistReq row);
        public Task<int> GetWishListCount(string client_id);
        #endregion

        #region "booking"
        public Task<int> GetMyBookingCount(string client_id);
        public Task<List<BookingSummary>> GetMyBooking(LangReq req, string client_id);
        public ResponseCls SaveClientBooking(BookingCls row);
        public List<TripFacility> GetTrip_Extra_Mains(TripExtraReq req);
        public List<TripFacility> getFacilityForTrip(long? trip_id, string lang_code, bool? isExtra,bool? is_obligatory);
        public ResponseCls AssignExtraToBooking(List<booking_extra> lst);
        public BookingPrice CalculateBookingPrice(CalculateBookingPriceReq req);
        public Task<BookingSummary> GetBookingWithDetails(BookingReq req);
        public Task<BookingWithTripDetailsAll> ConfirmBooking(ConfirmBookingReq req);
        public Task<ResponseCls> CancelBooking(long? booking_id, string? client_id);
        #endregion

        #region "Profile"
        public Task<List<ClientProfileCast>> GetClientProfiles(string clientId);
        public ResponseCls SaveMainProfile(ClientProfileCast profile);
        public Task<ResponseCls> SaveProfileImage(client_image image);
        public Task<List<client_image>> GetProfileImage(string clientId);
        public ResponseCls SaveClientNotificationSetting(client_notification_setting row);
        public Task<List<client_notification_setting>> GetClient_Notification_Settings(string clientId);
        #endregion

        #region "Contact"
        public ResponseCls SubscribeNewSletter(newsletter_subscriber row);
        #endregion
    }
}
