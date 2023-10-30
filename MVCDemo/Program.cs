using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks().AddCheck(
        "Foo Service",
        () =>
            {
                // Do your checks
                // ...
                return HealthCheckResult.Degraded("The check of the foo service did not work well");
            }, new[] { "services" })
    .AddCheck("Bar Service", () => HealthCheckResult.Healthy("The check of the bar service worked"), new[] { "services" })
    .AddCheck(
        "Database",
        () => HealthCheckResult.Healthy("The check of the database worked"), new[] { "services", "sql" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapHealthChecks(
    "/health",
    new HealthCheckOptions()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
