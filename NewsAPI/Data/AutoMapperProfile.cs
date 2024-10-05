using AutoMapper;
using NewsAPI.Models;

namespace NewsAPI.Data
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            //CreateMap<ContentType, M_SelectDropDown>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
        }
    }
}
