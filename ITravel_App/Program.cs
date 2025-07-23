using ITravel_App.Services;
using ITravelApp.Data;
using ITravelApp.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Serilog;

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("*")
                    .WithMethods("*")
                    .WithHeaders(HeaderNames.ContentType, "*");
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    Scheme = "bearer",
    //    BearerFormat = "JWT",
    //    In = ParameterLocation.Header,
    //    Name = "Authorization",
    //    Description = "Bearer Authentication with JWT Token",
    //    Type = SecuritySchemeType.Http
    //});
    //options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Id = "Bearer",
    //                Type = ReferenceType.SecurityScheme
    //            }
    //        },
    //        new List<string>()
    //    }
    //});
    options.OperationFilter<AcceptLanguageHeaderOperationFilter>();
});
builder.Services.AddDbContext<itravel_client_dbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ClientDAO>();
builder.Services.AddScoped<AdminDAO>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IClientService, ClientService>();
// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "de" }; // Add more as needed
    options.SetDefaultCulture("en");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//localization
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();
