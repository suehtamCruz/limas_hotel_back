using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace LimasHotel.Api.Services;

public class CloudinaryImageUploadService(Cloudinary cloudinary) : IImageUploadService
{
    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "limas-hotel/dogs"
        };

        var result = await cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            throw new InvalidOperationException($"Falha ao enviar imagem para o Cloudinary: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }
}
