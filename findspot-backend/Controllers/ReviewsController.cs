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

namespace findspot_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IAuthorizationHelperService _authService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserBlogPostRepository _userBlogPostRepository;

        public ReviewsController(
            IReviewRepository reviewRepository,
            IBlogPostRepository blogPostRepository,
            IAuthorizationHelperService authService,
            UserManager<User> userManager,
            IMapper mapper,
            IUserBlogPostRepository userBlogPostRepository)
        {
            _reviewRepository = reviewRepository;
            _blogPostRepository = blogPostRepository;
            _authService = authService;
            _userManager = userManager;
            _mapper = mapper;
            _userBlogPostRepository = userBlogPostRepository;
        }

        [HttpPost("{blogPostId}")]
        [Authorize]
        public async Task<IActionResult> AddReview(Guid blogPostId, [FromBody] ReviewCreateDto reviewCreateDto)
        {
            var blog = _blogPostRepository.Get(blogPostId);
            if (blog == null)
                return NotFound("Blog post not found");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            if (!_userBlogPostRepository.HasVisited(blogPostId, user.Id))
                return StatusCode(403, new { message = "You can only leave a review if you have visited this place." });

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Content = reviewCreateDto.Content,
                Rating = reviewCreateDto.Rating,
                BlogPostId = blogPostId,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                FeaturedImageUrl = reviewCreateDto.FeaturedImageUrl
            };

            _reviewRepository.Add(review);

            var responseDto = new ReviewResponseDto
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                DateAdded = review.DateAdded,
                UserName = user.UserName,
                FeaturedImageUrl = review.FeaturedImageUrl,
                BlogPost = new BlogPostSummaryDto
                {
                    Id = blog.Id,
                    PageTitle = blog.PageTitle,
                    ShortDescription = blog.ShortDescription,
                    FeaturedImageUrl = blog.FeaturedImageUrl,
                    PublishedDate = blog.PublishedDate
                }
            };

            return Ok(responseDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditReview(Guid id, [FromBody] ReviewCreateDto reviewDto)
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

            var blog = _blogPostRepository.Get(existing.BlogPostId);
            var user = await _userManager.FindByIdAsync(existing.UserId);

            var responseDto = new ReviewResponseDto
            {
                Id = existing.Id,
                Content = existing.Content,
                Rating = existing.Rating,
                DateAdded = existing.DateAdded,
                UserName = user?.UserName,
                FeaturedImageUrl = existing.FeaturedImageUrl,
                BlogPost = blog == null ? null : new BlogPostSummaryDto
                {
                    Id = blog.Id,
                    PageTitle = blog.PageTitle,
                    ShortDescription = blog.ShortDescription,
                    FeaturedImageUrl = blog.FeaturedImageUrl,
                    PublishedDate = blog.PublishedDate
                }
            };

            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize]
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
            var blog = _blogPostRepository.Get(blogPostId);

            var reviewResponseDtos = reviews.Select(review =>
            {
                var user = _userManager.FindByIdAsync(review.UserId).Result;

                return new ReviewResponseDto
                {
                    Id = review.Id,
                    Content = review.Content,
                    Rating = review.Rating,
                    DateAdded = review.DateAdded,
                    UserId = review.UserId,
                    UserName = user?.UserName,
                    FeaturedImageUrl = review.FeaturedImageUrl,
                    BlogPost = blog == null ? null : new BlogPostSummaryDto
                    {
                        Id = blog.Id,
                        PageTitle = blog.PageTitle,
                        ShortDescription = blog.ShortDescription,
                        FeaturedImageUrl = blog.FeaturedImageUrl,
                        PublishedDate = blog.PublishedDate
                    }
                };
            });

            return Ok(reviewResponseDtos);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = $"{StaticDetail.Role_Admin},{StaticDetail.Role_Moderator}")]
        public IActionResult GetReviewsByUser(string userId)
        {
            var reviews = _reviewRepository.GetAllByUserId(userId);

            var reviewResponseDtos = reviews.Select(review =>
            {
                var blog = _blogPostRepository.Get(review.BlogPostId);

                return new ReviewResponseDto
                {
                    Id = review.Id,
                    Content = review.Content,
                    Rating = review.Rating,
                    DateAdded = review.DateAdded,
                    UserName = null,
                    FeaturedImageUrl = review.FeaturedImageUrl,
                    BlogPost = blog == null ? null : new BlogPostSummaryDto
                    {
                        Id = blog.Id,
                        PageTitle = blog.PageTitle,
                        ShortDescription = blog.ShortDescription,
                        FeaturedImageUrl = blog.FeaturedImageUrl,
                        PublishedDate = blog.PublishedDate
                    }
                };
            });

            return Ok(reviewResponseDtos);
        }
    }
}
