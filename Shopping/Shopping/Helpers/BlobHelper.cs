using Microsoft.EntityFrameworkCore;
using Shopping.Data;


namespace Shopping.Helpers
{
    public class BlobHelper : IBlobHelper
        
    {
        private readonly DataContext _context;

        public BlobHelper(DataContext context)
        {
            _context = context;
        }
        public async Task DeleteBlobAsync(string id, string containerName)
        {

            if(File.Exists(id))
            {
                File.Delete(id);
            }
            
            
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string containerName)
        {
            
            // upload image to server
            string guid = Guid.NewGuid().ToString();
            string imagefile = $"{guid}.jpg";
            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"wwwroot\\images\\{containerName}",
                imagefile);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"images/{containerName}/{imagefile}";
        }

        public async Task<string> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            string guid = Guid.NewGuid().ToString();
            string imagefile = $"{guid}.jpg";

            try
            {
                stream.Position = 0;
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{containerName}", imagefile);
                File.WriteAllBytes(path, stream.ToArray());
            }
            catch
            {
                return string.Empty;
            }

            return $"images/{containerName}/{imagefile}";
        }

        public async Task<string> UploadBlobAsync(string image, string containerName)
        {
            string guid = Guid.NewGuid().ToString();
            image = $"{guid}.jpg";
            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"wwwroot\\images\\{containerName}",
                image);

            Stream stream = File.OpenRead(image);
            using (FileStream stream1 = new FileStream(path, FileMode.Create))
            {
                await stream.CopyToAsync(stream);
            }


            return $"images/{containerName}/{image}";
        }
    }
}
