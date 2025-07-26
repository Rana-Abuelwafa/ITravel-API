using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.trips;

namespace ITravel_App.Services
{
    public interface IClientService
    {
        #region destination
        public List<DestinationResponse> getDestinations(DestinationReq req);
        #endregion

        #region trips
        public Task<List<TripsAll>> GetTripsAll(TripsReq req);
        public Task<List<tripwithdetail>> GetTripsForSlider(TripsReq req);
        #endregion
    }
}
