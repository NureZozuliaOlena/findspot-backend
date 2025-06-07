using AutoMapper;
using findspot_backend.Models.DTO;
using findspot_backend.Models;

namespace findspot_backend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BlogPost, BlogPostDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<TouristObject, TouristObjectDto>().ReverseMap();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.IsLockedOut, opt =>
                    opt.MapFrom(src => src.LockoutEnd != null && src.LockoutEnd > DateTimeOffset.UtcNow));
            CreateMap<UserDto, User>();
            CreateMap<UserBlogPost, UserBlogPostDto>().ReverseMap();
        }
    }
}
