using ITravel_App.Models;
using ITravel_App.services;
using ITravel_App.Services;
using ITravelApp.Data.Entities;
using Mails_App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ITravel_App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TravelContactController : Controller
    {

        IMailService Mail_Service = null;
        private readonly IClientService _clientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginUserData _loginUserData;
      
        public TravelContactController(IClientService clientService,IMailService _MailService, IHttpContextAccessor httpContextAccessor)
        {
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
            _loginUserData = Utils.getTokenData(httpContextAccessor);
            Mail_Service = _MailService;
           
        }
        #region "contact"
        [HttpPost("SendContactMail")]
        public IActionResult SendContactMail(ContactReq req)
        {
           
            //string? clientId = _loginUserData?.client_id;
            //string? email = _loginUserData?.client_email;
            //string? fullName = _loginUserData?.FullName;
            string fileName = "ContactEmail_" + "en" + ".html";
            string htmlBody = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "MailsTemp//", fileName));
            var body = htmlBody
                        .Replace("{{Name}}", req.full_name)
                        .Replace("{{Email}}", req.email)
                        .Replace("{{Phone}}", req.phone ?? "-")
                        .Replace("{{Subject}}", req.subject)
                        .Replace("{{Message}}", req.message);
            MailData Mail_Data = new MailData { EmailBody = body, EmailSubject = "Contact Form Message" };

            return Ok(Mail_Service.ContactMail(Mail_Data, req.email));
        }

        [HttpPost("SubscribeNewSletter")]
        public IActionResult SubscribeNewSletter(newsletter_subscriber req)
        {

            string? clientId = _loginUserData?.client_id;
            //string? email = _loginUserData?.client_email;
            //string? fullName = _loginUserData?.FullName;
            req.client_id = clientId;
            return Ok(_clientService.SubscribeNewSletter(req));
     
        }
        #endregion
    }
}
