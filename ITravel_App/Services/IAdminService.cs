using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;

namespace ITravel_App.Services
{
    public interface IAdminService
    {
        public ResponseCls SaveMainDestination(destination_main row);
        public ResponseCls SaveDestinationTranslations(destination_translation row);
        public ResponseCls saveDestinationImage(destination_img row);
    }
}
