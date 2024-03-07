using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HalloDoc;
using HalloDoc.Repository.Interface;
using HalloDoc.Repository.Repository;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(720); // Set session timeout
});
builder.Services.AddScoped<IAdmin, HalloDoc.Repository.Repository.Admin>();
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CreateRequest}/{action=PatientSite}/{id?}");

app.Run();


