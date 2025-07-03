using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;

namespace Projet_7.Core.Mappings
{
    public class TradeProfile : Profile
    {
        public TradeProfile()
        {
            CreateMap<Trade, TradeDto>().ReverseMap();
        }
    }
}
