using AutoMapper;
using NewsWebsite.DTO;
using NewsWebsite.Models;

namespace NewsWebsite.Helpers
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Account, AccountDTO>().ReverseMap();
        }
    }
}
