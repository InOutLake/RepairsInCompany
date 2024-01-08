using Microsoft.EntityFrameworkCore;
using RepairsInCompany.Model.AuthDB;
using Microsoft.AspNetCore.Identity;
using RepairsInCompany.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using RepairsInCompany.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RepairsDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("RepairsDbConnection")));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbConnection")));
builder.Services
  .AddIdentityCore<IdentityUser>()
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Auth/Login";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.MapControllerRoute(
			name: "Auth",
			pattern: "Auth/{action=Index}/{id?}",
			defaults: new { controller = "Auth" }
		);

app.MapControllerRoute(
	name: "Main",
	pattern: "{controller=Main}/{action=ScheduleMonthGraph}");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
