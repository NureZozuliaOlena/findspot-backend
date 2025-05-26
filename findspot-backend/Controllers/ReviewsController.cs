using AutoMapper;
using findspot_backend.Models;
using findspot_backend.Models.DTO;
using findspot_backend.Repositories;
using findspot_backend.Repository;
using findspot_backend.Services;
using findspot_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IAuthorizationHelperService _authService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public ReviewsController(
            IReviewRepository reviewRepository,
            IBlogPostRepository blogPostRepository,
            IAuthorizationHelperService authService,
            UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _blogPostRepository = blogPostRepository;
            _authService = authService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost("{blogPostId}")]
        [Authorize(Roles = $"{StaticDetail.Role_User},{StaticDetail.Role_Admin},{StaticDetail.Role_Moderator}")]
        public async Task<IActionResult> AddReview(Guid blogPostId, [FromBody] ReviewDto reviewDto)
        {
            var blog = _blogPostRepository.Get(blogPostId);
            if (blog == null)
                return NotFound("Blog post not found");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var review = _mapper.Map<Review>(reviewDto);
            review.Id = Guid.NewGuid();
            review.BlogPostId = blogPostId;
            review.UserId = user.Id;
            review.DateAdded = DateTime.UtcNow;

            _reviewRepository.Add(review);

            return Ok(_mapper.Map<ReviewDto>(review));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_User},{StaticDetail.Role_Admin},{StaticDetail.Role_Moderator}")]
        public async Task<IActionResult> EditReview(Guid id, [FromBody] ReviewDto reviewDto)
        {
            var existing = _reviewRepository.GetById(id);
            if (existing == null)
                return NotFound();

            var (canEdit, _) = await _authService.CheckPermissionsAsync(existing.UserId, User);
            if (!canEdit)
                return Forbid();

            existing.Content = reviewDto.Content;
            existing.Rating = reviewDto.Rating;
            existing.FeaturedImageUrl = reviewDto.FeaturedImageUrl;

            _reviewRepository.Update(existing);

            return Ok(_mapper.Map<ReviewDto>(existing));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_User},{StaticDetail.Role_Admin},{StaticDetail.Role_Moderator}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null)
                return NotFound();

            var (_, canDelete) = await _authService.CheckPermissionsAsync(review.UserId, User);
            if (!canDelete)
                return Forbid();

            _reviewRepository.Delete(id);
            return NoContent();
        }

        [HttpGet("blog/{blogPostId}")]
        public IActionResult GetReviewsForBlogPost(Guid blogPostId)
        {
            var reviews = _reviewRepository.GetByBlogPostId(blogPostId);
            var dtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return Ok(dtos);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = $"{StaticDetail.Role_Admin},{StaticDetail.Role_Moderator}")]
        public IActionResult GetReviewsByUser(string userId)
        {
            var reviews = _reviewRepository.GetAllByUserId(userId);
            var dtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return Ok(dtos);
        }
    }
}
