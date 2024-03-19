using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RazeonProject.Data;
using RazeonProject.Filters;
using RazeonProject.Helpers;
using RazeonProject.Repositories.Classes;
using RazeonProject.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false);
builder.Services.AddSession();
builder.Services.AddTransient<HelperWwwroot>();
builder.Services.AddTransient<SendRazeonLayout>();
builder.Services.AddTransient<GlobalBuilderView>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

string connectionString = "";

#region SQL Server Conecction
connectionString = builder.Configuration.GetConnectionString("RazeonSQLServer") ?? "";
builder.Services.AddTransient<IRepositoryRazeonBBDD, RepositoryRazeonSQLServer>();
builder.Services.AddDbContext<ContextRazeonBBDD>
    (options => options.UseSqlServer(connectionString));
#endregion

var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Public}/{action=Index}/{id?}");
});

app.Run();
