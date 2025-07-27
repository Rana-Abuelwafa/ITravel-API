using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.trips;

namespace ITravel_App.Services
{
    public interface IAdminService
    {
        public ResponseCls SaveMainDestination(destination_main row);
        public ResponseCls SaveDestinationTranslations(destination_translation row);
        public ResponseCls saveDestinationImage(destination_img row);

        #region trips
        public ResponseCls SaveMainTrip(trip_main row);
        public ResponseCls SaveTripTranslation(TripTranslationReq row);
        public ResponseCls SaveTripPrices(TripPricesReq row);
        public ResponseCls saveTripImage(TripImgReq row);
        public ResponseCls SaveMainFacility(facility_main row);
        public ResponseCls SaveFacilityTranslation(FacilityTranslationReq row);
        public ResponseCls AssignFacilityToTrip(TripFacilityAssignReq row);

        public ResponseCls SaveMainTripPickups(TripsPickupSaveReq row);
        public ResponseCls SaveTripPickupsTranslations(TripsPickupTranslationSaveReq row);
      
        #endregion
    }
}
