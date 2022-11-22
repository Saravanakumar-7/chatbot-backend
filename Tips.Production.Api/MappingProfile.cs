

using AutoMapper;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities;
using Entities.DTOs;
using Entities;

namespace Tips.Production.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ShopOrder, ShopOrderDto>().ReverseMap();
            CreateMap<ShopOrder, ShopOrderDtoPost>().ReverseMap();
            CreateMap<ShopOrder, ShopOrderDtoUpdate>().ReverseMap();

            CreateMap<ShopOrderConfirmation, ShopOrderConfirmationDto>().ReverseMap();
            CreateMap<ShopOrderConfirmation, ShopOrderConfirmationDtoPost>().ReverseMap();
            CreateMap<ShopOrderConfirmation, ShopOrderConfirmationDtoUpdate>().ReverseMap();

        }
    }
}