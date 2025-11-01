using AchuBan_Ecom.DataAccess.Repository;
using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Data;
using AchuBan_ECom.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register EF Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ FIX: Add Identity with default token providers and UI
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Required for Razor Pages like Register/Login


// add configuration application cookie 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

// Register repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// ✅ FIX: Move exception handler outside of development check
app.UseExceptionHandler("/Home/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles(); // ✅ Required to serve static assets

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // ✅ Required for Identity UI (Register/Login pages)

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
