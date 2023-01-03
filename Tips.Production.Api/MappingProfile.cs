

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

            CreateMap<SAShopOrder, SAShopOrderDto>().ReverseMap();
            CreateMap<SAShopOrder, SAShopOrderDtoPost>().ReverseMap();
            CreateMap<SAShopOrder, SAShopOrderDtoUpdate>().ReverseMap();

            CreateMap<SAShopOrderMaterialIssue, SAShopOrderMaterialIssueDto>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssue, SAShopOrderMaterialIssueDtoPost>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssue, SAShopOrderMaterialIssueDtoUpdate>().ReverseMap();

            CreateMap<SAShopOrderMaterialIssueGeneral, SAShopOrderMaterialIssueGeneralDto>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssueGeneral, SAShopOrderMaterialIssueGeneralDtoPost>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssueGeneral, SAShopOrderMaterialIssueGeneralDtoUpdate>().ReverseMap();

            CreateMap<FGShopOrderMaterialIssue, FGShopOrderMaterialIssueDto>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssue, FGShopOrderMaterialIssueDtoPost>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssue, FGShopOrderMaterialIssueDtoUpdate>().ReverseMap();

            CreateMap<FGShopOrderMaterialIssueGeneral, FGShopOrderMaterialIssueGeneralDto>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssueGeneral, FGShopOrderMaterialIssueGeneralDtoPost>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssueGeneral, FGShopOrderMaterialIssueGeneralDtoUpdate>().ReverseMap();
        }
    }
}