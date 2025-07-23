using ITravelApp.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;

namespace ITravel_App.Services
{
    public class AdminService : IAdminService
    {
        private AdminDAO _adminDAO;

        public AdminService(AdminDAO adminDAO)
        {
            _adminDAO = adminDAO;

        }

        public ResponseCls saveDestinationImage(destination_img row)
        {
            return _adminDAO.saveDestinationImage(row);
        }

        public ResponseCls SaveDestinationTranslations(destination_translation row)
        {
            return _adminDAO.SaveDestinationTranslations(row);
        }

        public ResponseCls SaveMainDestination(destination_main row)
        {
            return _adminDAO.SaveMainDestination(row);
        }
    }
}
