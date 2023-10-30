using BlazorDemo.Data;
using BlazorDemo.HealthChecks;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHealthChecks()
    //.AddCheck(
    //    "Foo Service",
    //    () =>
    //        {
    //            // Do your checks
    //            // ...
    //            return HealthCheckResult.Degraded("The check of the foo service did not work well");
    //        }, new[] { "services" })
    //.AddCheck("Bar Service", () => HealthCheckResult.Healthy("The check of the bar service worked"), new[] { "services" })
    .AddCheck<ResponseTimeHealthCheck>("Network speed test", null, new[]{ "services" })
    .AddCheck(
        "Database",
        () => HealthCheckResult.Healthy("The check of the database worked"), new[] { "services", "sql" });
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<ResponseTimeHealthCheck>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapHealthChecks(
    "/health/services",
    new HealthCheckOptions()
        {
            Predicate = reg => reg.Tags.Contains("services"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
app.MapHealthChecks("/quickhealth", new HealthCheckOptions() { Predicate = _ => true });
app.MapHealthChecks(
    "/health",
    new HealthCheckOptions() { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
