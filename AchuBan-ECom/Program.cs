using AchuBan_Ecom.DataAccess.Repository;
using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing.Patterns;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Register repositories
//builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



var app = builder.Build();

// Development-only endpoint dump (put before middleware pipeline)
if (app.Environment.IsDevelopment())
{
    var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
    Console.WriteLine("=== Registered endpoints ===");
    foreach (var endpoint in endpointDataSource.Endpoints)
    {
        var routeEp = endpoint as RouteEndpoint;
        var pattern = routeEp?.RoutePattern?.RawText ?? "<no-route>";
        var httpMethods = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()?.HttpMethods;
        var cad = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        Console.WriteLine($"DisplayName: {endpoint.DisplayName} | Pattern: {pattern} | Methods: {string.Join(",", httpMethods ?? System.Array.Empty<string>())} | Controller: {cad?.ControllerTypeInfo.FullName} | Action: {cad?.ActionName} | Assembly: {cad?.ControllerTypeInfo.Assembly.FullName}");
    }
}

// Development-only endpoint dump (put before middleware pipeline)
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
