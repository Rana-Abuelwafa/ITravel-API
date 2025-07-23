using ITravel_App.Services;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.destination;
using ITravelApp.Data.Models.global;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITravel_App.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TravelAdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TravelAdminController(IHttpContextAccessor httpContextAccessor, IAdminService adminService)
        {
            _adminService = adminService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("SaveMainDestination")]
        public IActionResult SaveMainDestination(destination_main row)
        {

            return Ok(_adminService.SaveMainDestination(row));
        }
        [HttpPost("SaveDestinationTranslations")]
        public IActionResult SaveDestinationTranslations(destination_translation row)
        {

            return Ok(_adminService.SaveDestinationTranslations(row));
        }
        [HttpPost("saveDestinationImage")]
        public IActionResult saveDestinationImage([FromForm] DestinationImgReq req)
        {
            
            var path = Path.Combine("images" + "//", req.img.FileName);
            //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    req.img.CopyTo(stream);
                    stream.Close();
                }
               
            }
            catch (Exception ex)
            {
                
            }

            destination_img image = new destination_img
            {
                destination_id = req.destination_id,
                img_name = req.img.FileName,
                img_path = path,
                is_default = req.is_default,
                id=req.id
            };

            return Ok(_adminService.saveDestinationImage(image));
        }

    }
}
