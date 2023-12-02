using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddScoped<LayoutService>();
var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    "Default",
    "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    "Default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();
