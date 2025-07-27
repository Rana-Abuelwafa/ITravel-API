using Mails_App;
using static System.Net.WebRequestMethods;
using Travel_Authentication.Models;

namespace Travel_Authentication
{
    public class Utils
    {
        public static MailData GetOTPMailData(string lang, string fullName,string otp,string email)
        {
            string fileName = "OTPMail_" + lang + ".html";
            string htmlBody = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "MailsTemp//", fileName));
            htmlBody = htmlBody.Replace("@user", fullName);
            htmlBody = htmlBody.Replace("@otp", otp);
            MailData mailData = new MailData
            {
                EmailToId = email,
                EmailToName = email,
                EmailSubject = UtilsCls.GetMailSubjectByLang(lang, 2),
                EmailBody = htmlBody
            };
            return mailData;
        }
    }
}
