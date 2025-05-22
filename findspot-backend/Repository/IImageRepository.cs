namespace findspot_backend.Repository
{
    public interface IImageRepository
    {
        string Upload(IFormFile file);
    }
}