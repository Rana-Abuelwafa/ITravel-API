using ITravel_App.Models;
using ITravelApp.Data.Models.Bookings;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Linq.Expressions;

namespace ITravel_App.Services
{
    public class BookingPdfService
    {
        //public byte[] GenerateBookingPdf(BookingWithTripDetailsAll _model)
        //{
        //    Dictionary<string, string> L = Labels.Texts[_model.lang_code];
        //    var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logo.png");
        //    try 
        //    { 
        //    var document = Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Margin(20);
        //            page.Content().Column(col =>
        //            {
        //                // HEADER
        //                col.Item().PaddingBottom(20).BorderBottom(1).BorderColor("#e5e7eb").Row(row =>
        //                {
        //                    // row.RelativeItem().Image(logoPath, ImageScaling.FitHeight); // replace path
        //                    row.RelativeItem().Height(40).Image(logoPath);
        //                    row.RelativeItem().AlignRight().Column(c =>
        //                    {
        //                        c.Item().Text(L["Title"]).FontSize(15).FontColor("#5599cb");
        //                        c.Item().Text($"{L["Confirmed"]}!").FontSize(18).Bold();
        //                    });
        //                });

        //                // GREETING
        //                col.Item().PaddingVertical(20).Text($"{L["Greeting"]} {_model.client_name},")
        //                    .FontSize(16);

        //                col.Item().Text($"Thank you for choosing ITravel. " +
        //                                $"Your booking {_model.booking_code} for {_model.trip_name} is confirmed.")
        //                    .FontSize(13);

        //                // BOOKING DETAILS
        //                col.Item().PaddingTop(20).Border(1).BorderColor("#e5e7eb").CornerRadius(8)
        //                    .Table(table =>
        //                    {
        //                        table.ColumnsDefinition(cols => { cols.RelativeColumn(); cols.RelativeColumn(); });

        //                        table.Cell().Padding(10).Text($"{L["Trip"]}\n{_model.trip_name}");
        //                        table.Cell().Padding(10).AlignRight().Text($"{L["BookingRef"]}\n{_model.booking_code}");

        //                        table.Cell().Padding(10).Text($"{L["Date"]}\n{_model.trip_datestr}");
        //                        table.Cell().Padding(10).AlignRight()
        //                            .Text($"{L["Guests"]}\n{_model.total_pax} Adults - {_model.child_num} Children - {_model.infant_num} Infants");

        //                        table.Cell().Padding(10).Text($"{L["Pickup"]}\n{_model.pickup_address} at {_model.pickup_time}");
        //                        table.Cell().Padding(10).AlignRight().Text($"{L["Payment"]}\n Payment on site in cash {_model.total_price} {_model.currency_code}");
        //                    });

        //                // ITINERARY
        //                col.Item().PaddingVertical(20).Text(L["Itinerary"]).FontSize(16).Bold();
        //                col.Item().Border(1).BorderColor("#e5e7eb").CornerRadius(8).Table(table =>
        //                {
        //                    table.ColumnsDefinition(cols => cols.RelativeColumn());

        //                    foreach (var i in _model.pickups)
        //                    {
        //                        table.Cell().Padding(10).Column(c =>
        //                        {
        //                            c.Item().Text(i.pickup_name).Bold().FontSize(14);
        //                            c.Item().Text(i.duration).FontSize(12).FontColor("#555");
        //                        });
        //                    }
        //                });

        //                // PRICE SUMMARY
        //                col.Item().PaddingVertical(20).Text(L["PriceSummary"]).FontSize(16).Bold();
        //                col.Item().Border(1).BorderColor("#e5e7eb").CornerRadius(8).Table(table =>
        //                {
        //                    table.ColumnsDefinition(cols => { cols.RelativeColumn(); cols.ConstantColumn(100); });

        //                    table.Cell().Padding(10).Text(L["TripPrice"]);
        //                    table.Cell().AlignRight().Padding(10).Text($"{_model.total_price} {_model.currency_code}");

        //                    table.Cell().Padding(10).Text(L["Discount"]);
        //                    table.Cell().AlignRight().Padding(10).Text($"{0} {_model.currency_code}").FontColor("#5599cb");

        //                    table.Cell().BorderTop(1).BorderColor("#e5e7eb").Padding(10).Text(L["Total"]).Bold();
        //                    table.Cell().BorderTop(1).BorderColor("#e5e7eb").AlignRight().Padding(10)
        //                        .Text($"{_model.total_price - 0} {_model.currency_code}").Bold();
        //                });

        //                // FOOTER
        //                col.Item().PaddingTop(20).Text($"{L["Help"]} Info@expandhorizen.com or +2 0000000000")
        //                    .FontSize(11).FontColor("#555");
        //            });
        //        });


        //    });
        //    return document.GeneratePdf();
        //}
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}


        public async Task<byte[]> GenerateBookingPdf(BookingWithTripDetailsAll _model)
        {
            Dictionary<string, string> L = Labels.Texts[_model.lang_code];
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logo.png");

            try
            {
                return await Task.Run(() =>
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(20);
                            page.Content().Column(col =>
                            {
                                // HEADER
                                col.Item().PaddingBottom(20).BorderBottom(1).BorderColor("#e5e7eb").Row(row =>
                                {
                                    row.RelativeItem().Height(40).Image(logoPath);
                                    row.RelativeItem().AlignRight().Column(c =>
                                    {
                                        c.Item().Text(L["Title"]).FontSize(15).FontColor("#5599cb");
                                        c.Item().Text($"{L["Confirmed"]}!").FontSize(18).Bold();
                                    });
                                });

                                // GREETING
                                col.Item().PaddingVertical(20).Text($"{L["Greeting"]} {_model.client_name},")
                                    .FontSize(16);

                                col.Item().Text($"Thank you for choosing ITravel. " +
                                                $"Your booking {_model.booking_code} for {_model.trip_name} is confirmed.")
                                    .FontSize(13);

                                // BOOKING DETAILS
                                col.Item().PaddingTop(20).Border(1).BorderColor("#e5e7eb").CornerRadius(8)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(cols => { cols.RelativeColumn(); cols.RelativeColumn(); });
                                        table.Cell().Padding(8).Text($"{L["Trip"]}\n{_model.trip_name}");
                                        table.Cell().Padding(8).AlignRight().Text($"{L["BookingRef"]}\n{_model.booking_code}");
                                        table.Cell().Padding(8).Text($"{L["Date"]}\n{_model.trip_datestr}");
                                        table.Cell().Padding(8).AlignRight()
                                            .Text($"{L["Guests"]}\n{_model.total_pax} Adults - {_model.child_num} Children");

                                        if (_model.is_two_way == true)
                                        {
                                            table.Cell().Padding(8).Text($"{L["ReturnDate"]}\n{_model.trip_return_datestr}");
                                            table.Cell().Padding(8).AlignRight().Text("\n");
                                        }
                                        table.Cell().Padding(8)
                                         .Text($"{L["Pickup"]}\n{(string.IsNullOrWhiteSpace(_model.pickup_address) ? "N/A" : _model.pickup_address)}");
                                        //table.Cell().Padding(8).Text($"{L["Pickup"]}\n{_model.pickup_address}");
                                        table.Cell().Padding(8).AlignRight().Text($"{L["Nationality"]}\n{_model.client_nationality}");

                                        table.Cell().Padding(8).Text($"{L["Payment"]}\nPayment on site in cash {_model.total_price} {_model.currency_code}");
                                    });

                                // Extras
                                if ((_model.extras != null && _model.extras.Count > 0) || (_model.extras_obligatory != null && _model.extras_obligatory.Count > 0))
                                {
                                    col.Item().PaddingVertical(20).Text(L["Extra"]).FontSize(16).Bold();
                                    col.Item().Border(1).BorderColor("#e5e7eb").CornerRadius(8).Table(table =>
                                    {
                                        table.ColumnsDefinition(cols => cols.RelativeColumn());

                                        foreach (var i in _model.extras!)
                                        {
                                            table.Cell().Padding(10).Column(c =>
                                            {
                                                c.Item().Text($"({i.extra_count}) {i.extra_name}").Bold().FontSize(12);
                                                c.Item().Text($"{i.extra_price.ToString()} {_model.currency_code}").FontSize(12).FontColor("#555");
                                            });
                                        }

                                        foreach (var i in _model.extras_obligatory!)
                                        {
                                            table.Cell().Padding(10).Column(c =>
                                            {
                                                c.Item().Text($"({i.extra_count}) {i.extra_name}").Bold().FontSize(14);
                                                c.Item().Text($"{i.extra_price.ToString()} {_model.currency_code}").FontSize(14).FontColor("#555");
                                            });
                                        }
                                    });
                                }


                                // ITINERARY
                                if (_model.pickups != null && _model.pickups.Count > 0)
                                {
                                    col.Item().PaddingVertical(20).Text(L["Itinerary"]).FontSize(16).Bold();
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
                                }
                                // PRICE SUMMARY
                                col.Item().PaddingVertical(20).Text(L["PriceSummary"]).FontSize(16).Bold();
                                col.Item().Border(1).BorderColor("#e5e7eb").CornerRadius(8).Table(table =>
                                {
                                    table.ColumnsDefinition(cols => { cols.RelativeColumn(); cols.ConstantColumn(100); });

                                    table.Cell().Padding(10).Text(L["TripPrice"]);
                                    table.Cell().AlignRight().Padding(10).Text($"{_model.total_price} {_model.currency_code}");

                                    table.Cell().Padding(10).Text(L["Discount"]);
                                    table.Cell().AlignRight().Padding(10).Text($"{0} {_model.currency_code}").FontColor("#5599cb");

                                    table.Cell().BorderTop(1).BorderColor("#e5e7eb").Padding(10).Text(L["Total"]).Bold();
                                    table.Cell().BorderTop(1).BorderColor("#e5e7eb").AlignRight().Padding(10)
                                        .Text($"{_model.total_price - 0} {_model.currency_code}").Bold();
                                });

                                // FOOTER
                                col.Item().PaddingTop(20).Text($"{L["Help"]} Info@itravel.com or +2 0000000000")
                                    .FontSize(11).FontColor("#555");
                            });
                        });
                    });

                    return document.GeneratePdf();
                });
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
