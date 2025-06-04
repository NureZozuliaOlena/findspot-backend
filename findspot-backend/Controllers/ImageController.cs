using Microsoft.AspNetCore.Mvc;
using System.Net;
using findspot_backend.Repository;

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly IImageRepository imageRepository;

        public ImageController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            var imageUrl = imageRepository.Upload(file);

            if (imageUrl == null)
            {
                return Problem("Something went wrong!", null, (int)HttpStatusCode.InternalServerError);
            }

            return Json(new { link = imageUrl });
        }
    }
}