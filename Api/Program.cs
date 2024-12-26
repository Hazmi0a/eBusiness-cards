using Microsoft.AspNetCore.Mvc;
using QRCodePOC;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add custom services here
builder.Services.AddScoped<QRCode>(); // Register the QRCode class
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapGet("/VcardQRcodeRaw", async ([FromServices] QRCode qrCodeService, [FromQuery] string email, [FromQuery] string fn, [FromQuery] string n, [FromQuery] string tel, [FromQuery] string title) =>
{
    var vcard =  await qrCodeService.VcardGenerator(email, fn, n, tel, title);
    var data = await qrCodeService.GenerateQrCodeNative(vcard);
    return "data:image/png;base64,"+data;
});

app.Run();

