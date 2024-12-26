using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp;

namespace QRCodePOC
{
    public class QRCode
    {
        private readonly ILogger<QRCode> _logger;

        public QRCode(ILogger<QRCode> logger)
        {
            _logger = logger;
        }
        
        
        public async Task<string> VcardGenerator(string email, string fn, string n, string tel, string title)
        {
            _logger.LogInformation("generating QR code for {email}", email);
            return "BEGIN:VCARD\n" +
                   "VERSION:4.0\n" +
                   $"FN:{fn}\n" +
                   $"N:{n}\n" +
                   $"TEL;TYPE=WORK,VOICE:{tel}\n" +
                   $"EMAIL:{email}\n" +
                   "ORG:New Murabba\n" +
                   $"TITLE:{title}\n" +
                   $"ROLE:{title}\n" +
                   "URL:https://newmurabba.com\n" +
                   "END:VCARD";
        }

        public async Task<string> GenerateQrCodeNative(string content)
        {
            const int widthPixels = 1000;
            const int heightPixels = 1000;
            var qrCodeContent = content;

            BarcodeWriter qrWriter = new()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = heightPixels,
                    Width = widthPixels,
                    Margin = 0,
                },
            };

            using SKBitmap qrCode = qrWriter.Write(qrCodeContent);
            using SKData encoded = qrCode.Encode(SKEncodedImageFormat.Png, 100);
            var imageData = Convert.ToBase64String(encoded.AsSpan());
            return imageData;
        }
        
        
    }
}
