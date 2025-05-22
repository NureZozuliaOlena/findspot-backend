using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace findspot_backend.Repository
{
    public class ImageRepository : IImageRepository
    {
        private readonly Account account;

        public ImageRepository(IConfiguration configuration)
        {
            account = new Account(configuration.GetSection("Cloudinary")["CloudName"],
                                  configuration.GetSection("Cloudinary")["ApiKey"],
                                  configuration.GetSection("Cloudinary")["ApiSecret"]);
        }

        public string Upload(IFormFile file)
        {
            var client = new Cloudinary(account);
            var uploadFileResult = client.Upload(
                new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    DisplayName = file.FileName
                });

            if (uploadFileResult != null && uploadFileResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadFileResult.SecureUri.ToString();
            }

            return null;
        }
    }
}