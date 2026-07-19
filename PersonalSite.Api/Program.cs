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

var app = builder.Build();
app.UsePersonalSite();
app.UseCors();
app.Run();

public partial class Program;