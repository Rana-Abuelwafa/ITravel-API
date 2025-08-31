using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.trips;

namespace ITravel_App.Services
{
    public interface IAdminService
    {
        public ResponseCls SaveMainDestination(destination_main row);
        public ResponseCls SaveDestinationTranslations(destination_translation row);
        public ResponseCls saveDestinationImage(List<destination_img> row);
        public List<DestinationWithTranslations> GetDestinationWithTranslations(DestinationReq req);
        public Task<List<destination_img>> GetImgsByDestination(int? destination_id);
        public Task<List<destination_main>> GetDestination_Mains();
        public ResponseCls UpdateDestinationImage(DestinationImgUpdateReq cls);


        #region trips
        public Task<List<trip_category>> GetTripCategories();
        public Task<List<trip_price>> GetTrip_Prices(long? trip_id);
        public Task<List<TripTranslationGrp>> GetTripTranslationGrp(long? trip_id);
        public ResponseCls SaveMainTrip(trip_main row);
        public ResponseCls SaveTripTranslation(TripTranslationReq row);
        public ResponseCls SaveTripPrices(TripPricesReq row);
        public ResponseCls saveTripImage(List<trip_img> lst);
        public ResponseCls SaveMainFacility(facility_main row);
        public ResponseCls SaveFacilityTranslation(FacilityTranslationReq row);
        public ResponseCls AssignFacilityToTrip(TripFacilityAssignReq row);

        public ResponseCls SaveMainTripPickups(TripsPickupSaveReq row);
        public ResponseCls SaveTripPickupsTranslations(TripsPickupTranslationSaveReq row);

        public Task<List<TripMainCast>> GetTrip_Mains(int destination_id, int trip_type);

        public List<TripsPickupResponseGrp> GetPickupsAllForTrip(PickupsReq req);
        public Task<List<trip_img>> GetImgsByTrip(decimal? trip_id);
        public ResponseCls UpdateTripImage(TripImgUpdateReq trip);

        public List<FacilityWithTranslationGrp> GetFacilityWithTranslation();

        public List<FacilityAllWithSelect> GetFacilityAllWithSelect(long? trip_id);
        #endregion
    }
}
