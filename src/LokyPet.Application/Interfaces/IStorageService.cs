namespace LokyPet.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadProductImageAsync(Stream file, string fileName);
    Task DeleteProductImageAsync(string path);
    string GetPublicUrl(string path);
}
