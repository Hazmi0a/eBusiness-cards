using QRCodePOC.Dtos;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp;

namespace QRCodePOC
{
    public class QRCode : IQRCode
    {
        private readonly ILogger<QRCode> _logger;
        private readonly IConfiguration _configuration;

        public QRCode(ILogger<QRCode> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        
        
        public async Task<string?> VcardGenerator(string email, string fn, string n, string tel, string title)
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

        public async Task<string> GenerateQrCodeNative(string? content)
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

            // return encoded;
            var imageData = Convert.ToBase64String(encoded.AsSpan());
            return imageData;
        }

        public async Task<string> SaveQrCode(string imageBase64, string email)
        {
            byte[] imageByteArray = Convert.FromBase64String(imageBase64);
            
            // Ensure the directory exists
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "QRCodes");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Generate the file path
            var fileName = $"{email}_QRCode_{Guid.NewGuid()}.png";
            var filePath = Path.Combine(directoryPath, fileName);

            // Save the file to the directory
            await File.WriteAllBytesAsync(filePath, imageByteArray);
            // await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            // imageBase64.SaveTo(fileStream);

            // Return the file path or name (optional)
            return fileName;
            
        }

        public async Task<Response?> GetUrl(string email)
        {
            _logger.LogInformation("getting QR code url for {email}", email);
            var hostname = _configuration.GetValue<string>("HostName");
            // Check if there's an existing QR code file in the QRCodes folder
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "QRCodes");
            var existingFile = Directory
                .GetFiles(folderPath, $"{email}*")
                .FirstOrDefault();


            if (existingFile == null)
            {
                _logger.LogInformation("No QR code found for {email}", email);
                return null;
            }
            
            // Check if the file is more than 2 weeks old
            if (existingFile != null)
            {
                _logger.LogInformation("QR code found for {email}, but its older than 2 weeks. deleting & generating new", email);
                var fileInfo = new FileInfo(existingFile);
                var olderThan = int.TryParse(_configuration["OlderThan"], out var value) ? value : 14;
                
                if (fileInfo.CreationTimeUtc < DateTime.UtcNow.AddDays(olderThan))
                {
                    // File is older than 2 weeks, delete it
                    File.Delete(existingFile);
                    return null;
                }
            }
            
            _logger.LogInformation("QR code found for {email}", email);
            var fileName = Path.GetFileName(existingFile);
            var res = new Response() { Url = hostname + "/QRCodes/" + fileName };
            return res; // Return the existing file path if found
        }
        
        
    }
}
