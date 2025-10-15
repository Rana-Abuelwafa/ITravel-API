namespace ITravel_App.Models
{
    public static class Labels
    {
        public static Dictionary<string, Dictionary<string, string>> Texts = new()
        {
            ["en"] = new Dictionary<string, string>
            {
                ["Title"] = "BOOKING CONFIRMATION",
                ["Greeting"] = "Hi",
                ["Confirmed"] = "Your trip is confirmed",
                ["Trip"] = "Trip",
                ["BookingRef"] = "Booking Ref",
                ["Date"] = "Date",
                ["Guests"] = "Guests",
                ["Pickup"] = "Pickup",
                ["Payment"] = "Payment",
                ["Itinerary"] = "Itinerary",
                ["PriceSummary"] = "Price Summary",
                ["TripPrice"] = "Trip price",
                ["Discount"] = "Discount",
                ["Total"] = "Total",
                ["Help"] = "Need help? Contact us at",
                ["Nationality"]= "Nationality",
                ["ReturnDate"] = "Return Date",
                ["Extra"] = "Extras"
            },
            ["de"] = new Dictionary<string, string>
            {
                ["Title"] = "REISEBESTÄTIGUNG",
                ["Greeting"] = "Hallo",
                ["Confirmed"] = "Ihre Reise ist bestätigt",
                ["Trip"] = "Reise",
                ["BookingRef"] = "Buchungsnr.",
                ["Date"] = "Daten",
                ["Guests"] = "Gäste",
                ["Pickup"] = "Abholung",
                ["Payment"] = "Zahlung",
                ["Itinerary"] = "Reiseplan",
                ["PriceSummary"] = "Preisübersicht",
                ["TripPrice"] = "Reisepreis",
                ["Discount"] = "Rabatt",
                ["Total"] = "Gesamt",
                ["Help"] = "Brauchen Sie Hilfe? Kontaktieren Sie uns unter",
                ["Nationality"] = "Nationalität",
                ["ReturnDate"] = "Rückflugdatum",
                ["Extra"] = "Extras"
            }
        };
    }

}
