using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravel_App
{
    public class UtilsCls
    {
        //this type config is static in code,
        //1 = confirmation mail ,
        //2 = otp verify
        //3 = invoice mail
        //4 = CUSTOMER SUPPORT mail

        public static string GetMailSubjectByLang(string lang,int type)
        {
            
            if (type == 1)
            //mean confirmation mail
            {
                if (lang == "ar")
                    return "مرحباً بك في هوريزون";
                else if (lang == "en")
                    return "Welcome to ITravel !";
                else if (lang == "de")
                    return "Willkommen bei ITravel";
                else return "";
            }
            else if (type == 2)
            {
                //mean otp verify
                if (lang == "ar")
                    return "تأكيد البريد الإلكتروني-هوريزون";
                else if (lang == "en")
                    return "ITravel - Verify Your Email";
                else if (lang == "de")
                    return "ITravel - Bestätigen Sie Ihre E-Mail";
                else return "";
            }
            else if (type == 3)
            {
                //mean booking confirmation to client
                if (lang == "ar")
                    return "   فاتوره - هوريزون";
                else if (lang == "en")
                    return "ITravel - Booking Confirmation";
                else if (lang == "de")
                    return "ITravel - Buchungsbestätigung";
                else return "";
            }
            else if (type == 4)
            {
                //mean invoice
                if (lang == "ar")
                    return "   خدمة عملاء - هوريزون";
                else if (lang == "en")
                    return "ITravel - Customer Support";
                else if (lang == "de")
                    return "ITravel - Klantenondersteuning";
                else return "";
            }
           
            else return "";
            
        }
    }
}
