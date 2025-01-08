using QRCodePOC.Dtos;

namespace QRCodePOC;

public interface IQRCode
{
    public Task<string?> VcardGenerator(string email, string fn, string n, string tel, string title);
    public Task<string> GenerateQrCodeNative(string? content);
    public Task<string> SaveQrCode(string imageBase64, string email);
    public Task<Response?> GetUrl(string email);

}