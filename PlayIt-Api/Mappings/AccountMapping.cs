using AutoMapper;
using PlayIt_Api.Models.Dto;

namespace PlayIt_Api.Mappings
{
    public class AccountMapping : Profile
    {
        public AccountMapping()
        {
            CreateMap<Account, Models.Entities.Account>().ReverseMap();
        }
    }
}
