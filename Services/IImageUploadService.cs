namespace LimasHotel.Api.Services;

public interface IImageUploadService
{
    Task<string> UploadAsync(Stream fileStream, string fileName);
}
