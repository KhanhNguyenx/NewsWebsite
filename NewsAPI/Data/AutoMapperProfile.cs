using AutoMapper;

namespace NewsAPI.Data
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<Account, AccountDTO>().ReverseMap();

            //CreateMap<ContentType, M_SelectDropDown>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
        }
    }
}
