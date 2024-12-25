using Microsoft.AspNetCore.Mvc;
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

app.MapGet("/VcardQRcodeRaw", ([FromQuery] string email, [FromQuery] string fn, [FromQuery] string n, [FromQuery] string tel, [FromQuery] string title) =>
{
    string vcard =  QRCode.VcardGenerator(email, fn, n, tel, title);
    var data = QRCode.GenerateQrCodeNative(vcard);
    return "data:image/png;base64,"+data;
});

// app.MapGet("vcard", (string encodedVCard) =>
// {
//     string decodedVCard = WebUtility.UrlDecode(encodedVCard);
//
//     return decodedVCard;
// });
//
// app.MapGet("working", (string encodedVCard) =>
// {
//     return QRCode.EncodeVcard();
// });

app.Run();

