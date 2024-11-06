using AutoMapper;
using NewsAPI.Models;
using NewsAPI.DTOs;

namespace NewsAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<RoleUser,RoleUserDTO>().ReverseMap();
            CreateMap<Comment, CommentDTO>().ReverseMap();
            CreateMap<UserProfile,UserProfileDTO>().ReverseMap();
            CreateMap<UserPost, UserPostDTO>().ReverseMap();
            CreateMap<Image, ImageDTO>().ReverseMap();
            CreateMap<Post,PostDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            //CreateMap<ContentType, M_SelectDropDown>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
        }
    }
}
