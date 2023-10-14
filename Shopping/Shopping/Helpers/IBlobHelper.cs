namespace Shopping.Helpers
{
    public interface IBlobHelper
    {
        Task<String> UploadBlobAsync(IFormFile file, string containerName);

        Task<String> UploadBlobAsync(byte[] file, string containerName);

        Task<String> UploadBlobAsync(string image, string containerName);

        Task DeleteBlobAsync(string id, string containerName);
    }
}
