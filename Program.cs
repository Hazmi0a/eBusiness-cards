using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using QRCodePOC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/GenerateQRCode", (string text) =>
{
    //generate guid
    var guid = Guid.NewGuid().ToString();

    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    QRCodeData qrCodeData = qrGenerator.CreateQrCode(text + "\r\n" + guid, QRCodeGenerator.ECCLevel.Q);
    QRCode qrCode = new QRCode(qrCodeData);
    Bitmap qrCodeImage = qrCode.GetGraphic(20);
    // use this when you want to show your logo in middle of QR Code and change color of qr code
    Bitmap logoImage = new Bitmap(@"wwwroot/img/aircodlogo.jpg");
    // Generate QR Code bitmap and convert to Base64
    using (Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20, Color.Black, Color.WhiteSmoke, logoImage))
    {
        using (MemoryStream ms = new MemoryStream())
        {
            qrCodeAsBitmap.Save(ms, ImageFormat.Png);
            string base64String = Convert.ToBase64String(ms.ToArray());
            var data = "data:image/png;base64," + base64String;
            return data;
        }
    }
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
