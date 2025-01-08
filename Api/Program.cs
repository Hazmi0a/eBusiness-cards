using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using QRCodePOC;
using QRCodePOC.Auth;
using QRCodePOC.Dtos;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add custom services here
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IQRCode, QRCode_dev>(); // Register the Dev QRCode class
}
else
{
    builder.Services.AddScoped<IQRCode, QRCode>(); // Register the QRCode class
}

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
// Log the environment during application startup
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

app.UseSwagger();
app.UseSwaggerUI();
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
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "QRCodes")),
    RequestPath = "/qrcodes"
});

app.UseRouting();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapGet("/GenerateQrCode", async ([FromServices] IQRCode qrCodeService, [FromQuery] string email, [FromQuery] string fn, [FromQuery] string n, [FromQuery] string tel, [FromQuery] string title) =>
{
    // validate that the emails end with new murabba email 
    if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@newmurabba\.com$"))
    {
        // return TypedResults.BadRequest("Invalid email. Only emails ending with @newmurabba.com are allowed.");
        return Results.BadRequest("Invalid email. Only emails ending with @newmurabba.com are allowed.");
    }
    var vcard =  await qrCodeService.VcardGenerator(email, fn, n, tel, title);
    var data = await qrCodeService.GenerateQrCodeNative(vcard);
    
    // Check if there's an existing QR code file in the wwwroot/qrcodes folder
    var url = await qrCodeService.GetUrl(email);
    if (url != null) return Results.Ok(url);

    // If no file is found, execute SaveQrCode
    var qrcodeUrl = await qrCodeService.SaveQrCode(data, email);
    url = await qrCodeService.GetUrl(email);
    
    return Results.Ok(url);
    
    // return "data:image/png;base64," + data;
    // return Results.File(Convert.FromBase64String(data), "image/png", "qrcode.png");
});

app.Run();

