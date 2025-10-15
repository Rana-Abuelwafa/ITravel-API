using ITravel_App.Models;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.Bookings.Client;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class BookingPdfDocument : IDocument
{
    private readonly BookingWithTripDetailsAll _model;
    private readonly Dictionary<string, string> L;
    public BookingPdfDocument(BookingWithTripDetailsAll model)
    {
        _model = model;
        L = Labels.Texts[_model.lang_code];
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logo.png");
        container.Page(page =>
        {
            page.Margin(20);
            page.Content().Column(col =>
            {
                // HEADER
                col.Item().Row(row =>
                {
                    row.RelativeItem().Image(logoPath, ImageScaling.FitHeight); // replace path
                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text(L["Title"]).FontSize(12).FontColor("#5599cb");
                        c.Item().Text($"{L["Confirmed"]}, {_model.client_name}!").FontSize(20).Bold();
                    });
                });

                // GREETING
                col.Item().PaddingTop(20).Text($"{L["Greeting"]} {_model.client_name},")
                    .FontSize(16);

                col.Item().Text($"Thank you for choosing ITravel. " +
                                $"Your booking {_model.booking_code} for {_model.trip_name} is confirmed.")
                    .FontSize(14);

                // BOOKING DETAILS
                col.Item().PaddingTop(20).Border(1).BorderColor("#e5e7eb").CornerRadius(8)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(cols => { cols.RelativeColumn(); cols.RelativeColumn(); });

                        table.Cell().Padding(10).Text($"{L["Trip"]}\n{_model.trip_name}");
                        table.Cell().Padding(10).AlignRight().Text($"{L["BookingRef"]}\n{_model.booking_code}");

                        table.Cell().Padding(10).Text($"{L["Date"]}\n{_model.trip_datestr}");
                        table.Cell().Padding(10).AlignRight()
                            .Text($"{L["Guests"]}\n{_model.total_pax} Adults - {_model.child_num} Children - {_model.infant_num} Infants");

                        table.Cell().Padding(10).Text($"{L["Pickup"]}\n{_model.pickup_address} at {_model.pickup_time}");
                        table.Cell().Padding(10).AlignRight().Text($"{L["Payment"]}\n Payment on site in cash {_model.currency_code} {_model.total_price}");
                    });

                // ITINERARY
                col.Item().PaddingTop(20).Text(L["Itinerary"]).FontSize(16).Bold();
                col.Item().Border(1).BorderColor("#e5e7eb").CornerRadius(8).Table(table =>
                {
                    table.ColumnsDefinition(cols => cols.RelativeColumn());

                    foreach (var i in _model.pickups)
                    {
                        table.Cell().Padding(10).Column(c =>
                        {
                            c.Item().Text(i.pickup_name).Bold().FontSize(14);
                            c.Item().Text(i.duration).FontSize(12).FontColor("#555");
                        });
                    }
                });

                // PRICE SUMMARY
                col.Item().PaddingTop(20).Text(L["PriceSummary"]).FontSize(16).Bold();
                col.Item().Border(1).BorderColor("#e5e7eb").CornerRadius(8).Table(table =>
                {
                    table.ColumnsDefinition(cols => { cols.RelativeColumn(); cols.ConstantColumn(100); });

                    table.Cell().Padding(10).Text(L["TripPrice"]);
                    table.Cell().AlignRight().Padding(10).Text($"{_model.currency_code}{_model.total_price}");

                    table.Cell().Padding(10).Text(L["Discount"]);
                    table.Cell().AlignRight().Padding(10).Text($"{_model.currency_code}{0}").FontColor("#5599cb");

                    table.Cell().BorderTop(1).BorderColor("#e5e7eb").Padding(10).Text(L["Total"]).Bold();
                    table.Cell().BorderTop(1).BorderColor("#e5e7eb").AlignRight().Padding(10)
                        .Text($"{_model.currency_code}{_model.total_price - 0}").Bold();
                });

                // FOOTER
                col.Item().PaddingTop(20).Text($"{L["Help"]} Info@itravel.com or +2 0000000000")
                    .FontSize(11).FontColor("#555");
            });
        });
    }
}


