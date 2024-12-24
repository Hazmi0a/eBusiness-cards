using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
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

app.MapGet("/VcardQRcode", (string encodedVCard) =>
{
    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    string decodedVCard = WebUtility.UrlDecode(encodedVCard);
    QRCodeData qrCodeData = qrGenerator.CreateQrCode(decodedVCard, QRCodeGenerator.ECCLevel.Q);
    QRCode qrCode = new QRCode(qrCodeData);
    Bitmap qrCodeImage = qrCode.GetGraphic(20);
    // use this when you want to show your logo in middle of QR Code and change color of qr code
    Bitmap logoImage = new Bitmap(@"wwwroot/img/NMDC_Logo.png");
    // Generate QR Code bitmap and convert to Base64
    using (Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20, Color.Black, Color.WhiteSmoke,null))
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

app.MapGet("vcard", (string encodedVCard) =>
{
    string decodedVCard = WebUtility.UrlDecode(encodedVCard);

    return decodedVCard;
});

app.MapGet("working", (string encodedVCard) =>
{
    return QRCode.EncodeVcard();
});

app.Run();

