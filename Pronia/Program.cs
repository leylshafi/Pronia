using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Interfaces;
using Pronia.Models;
using Pronia.Services;
using Pronia.ViewComponents;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddViewComponentsAsServices(); ;
builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase= true;
    options.Password.RequireLowercase= false;

    options.User.RequireUniqueEmail = false;

    options.Lockout.MaxFailedAccessAttempts= 3;
    options.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(3);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<HeaderViewComponent>();
builder.Services.AddScoped<IEmailService,EmailService>();
var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    "Default",
    "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    "Default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();
