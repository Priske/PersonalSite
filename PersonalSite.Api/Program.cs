using System.Text.Json.Serialization;
using PersonalSite.Api.Wiring;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationServices();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCanonicalHostRedirect();
app.UseCors();
app.UsePersonalSite();
app.MapFallbackToFile("index.html");
app.Run();

public partial class Program;