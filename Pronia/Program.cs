using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
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
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    "Default",
    "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    "Default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();
