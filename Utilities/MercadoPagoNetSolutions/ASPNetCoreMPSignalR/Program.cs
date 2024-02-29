using ASPNetCoreMPSignalR;
using IASPNetCoreMPSignalR.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddScoped<ChatHub>();
builder.Services.AddScoped<NotificationController>();

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});
var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");
app.UseAuthorization();
app.MapControllers();
app.Run();
