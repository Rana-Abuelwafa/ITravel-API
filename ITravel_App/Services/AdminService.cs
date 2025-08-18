using ITravelApp.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.trips;

namespace ITravel_App.Services
{
    public class AdminService : IAdminService
    {
        private AdminDAO _adminDAO;

        public AdminService(AdminDAO adminDAO)
        {
            _adminDAO = adminDAO;

        }

        public ResponseCls AssignFacilityToTrip(TripFacilityAssignReq row)
        {
            return _adminDAO.AssignFacilityToTrip(row);
        }

        public List<DestinationWithTranslations> GetDestinationWithTranslations(DestinationReq req)
        {
            return _adminDAO.GetDestinationWithTranslations(req);
        }

        public Task<List<destination_main>> GetDestination_Mains()
        {
            return _adminDAO.GetDestination_Mains();
        }

        public Task<List<destination_img>> GetImgsByDestination(int? destination_id)
        {
            return _adminDAO.GetImgsByDestination(destination_id);
        }

        public Task<List<TripMainCast>> GetTrip_Mains(int destination_id)
        {
            return _adminDAO.GetTrip_Mains(destination_id);
        }

        public ResponseCls saveDestinationImage(destination_img row)
        {
            return _adminDAO.saveDestinationImage(row);
        }

        public ResponseCls SaveDestinationTranslations(destination_translation row)
        {
            return _adminDAO.SaveDestinationTranslations(row);
        }

        public ResponseCls SaveFacilityTranslation(FacilityTranslationReq row)
        {
            return _adminDAO.SaveFacilityTranslation(row);
        }

        public ResponseCls SaveMainDestination(destination_main row)
        {
            return _adminDAO.SaveMainDestination(row);
        }

        public ResponseCls SaveMainFacility(facility_main row)
        {
            return _adminDAO.SaveMainFacility(row);
        }

        public ResponseCls SaveMainTrip(trip_main row)
        {
            return _adminDAO.SaveMainTrip(row);
        }

        public ResponseCls SaveMainTripPickups(TripsPickupSaveReq row)
        {
            return _adminDAO.SaveMainTripPickups(row);
        }

       
        public ResponseCls saveTripImage(TripImgReq row)
        {
            return _adminDAO.saveTripImage(row);
        }

        public ResponseCls SaveTripPickupsTranslations(TripsPickupTranslationSaveReq row)
        {
            return _adminDAO.SaveTripPickupsTranslations(row);
        }

        public ResponseCls SaveTripPrices(TripPricesReq row)
        {
            return _adminDAO.SaveTripPrices(row);
        }

        public ResponseCls SaveTripTranslation(TripTranslationReq row)
        {
            return _adminDAO.SaveTripTranslation(row);
        }
    }
}
