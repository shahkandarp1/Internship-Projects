using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HalloDoc;
using HalloDoc.Hubs;
using HalloDoc.Repository.Interface;
using HalloDoc.Repository.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(720); // Set session timeout
});
builder.Services.AddScoped<IAdmin, HalloDoc.Repository.Repository.Admin>();
builder.Services.AddScoped<IDoctor, Doctor>();
builder.Services.AddScoped<IPatient, HalloDoc.Repository.Repository.Patient>();
builder.Services.AddScoped<IJwtService, HalloDoc.Repository.Repository.JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseRotativa();

app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CreateRequest}/{action=PatientSite}/{id?}");

app.MapHub<ChatHub>("/chathub");

app.Run();


