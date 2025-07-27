

using Mails_App;

namespace Travel_Authentication.Services
{
    public interface IMailService
    {
        bool SendMail(MailData Mail_Data);
        //bool SendOTPMail(MailData Mail_Data);
        
    }

}
