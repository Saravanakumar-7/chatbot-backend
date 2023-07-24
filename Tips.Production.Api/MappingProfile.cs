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
            CreateMap<ShopOrder, ShopOrderPostDto>().ReverseMap();
            CreateMap<ShopOrder, ShopOrderUpdateDto>().ReverseMap();
            CreateMap<ShopOrder, ListOfShopOrderDto>().ReverseMap();
            
            CreateMap<ShopOrderItem, ShopOrderItemDto>().ReverseMap();
            CreateMap<ShopOrderItem, ShopOrderItemPostDto>().ReverseMap();
            CreateMap<ShopOrderItem, ShopOrderItemUpdateDto>().ReverseMap();
            CreateMap<ShopOrderItemPostDto, UpdateSalesOrderQtyDto>().ReverseMap();

            CreateMap<ShopOrderConfirmation, ShopOrderConfirmationDto>().ReverseMap();
            CreateMap<ShopOrderConfirmation, ShopOrderConfirmationPostDto>().ReverseMap();
            CreateMap<ShopOrderConfirmation, ShopOrderConfirmationUpdateDto>().ReverseMap();

            CreateMap<SAShopOrder, SAShopOrderDto>().ReverseMap();
            CreateMap<SAShopOrder, SAShopOrderPostDto>().ReverseMap();
            CreateMap<SAShopOrder, SAShopOrderUpdateDto>().ReverseMap();

            CreateMap<SAShopOrderMaterialIssue, SAShopOrderMaterialIssueDto>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssue, SAShopOrderMaterialIssuePostDto>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssue, SAShopOrderMaterialIssueUpdateDto>().ReverseMap();

            CreateMap<SAShopOrderMaterialIssueGeneral, SAShopOrderMaterialIssueGeneralDto>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssueGeneral, SAShopOrderMaterialIssueGeneralPostDto>().ReverseMap();
            CreateMap<SAShopOrderMaterialIssueGeneral, SAShopOrderMaterialIssueGeneralUpdateDto>().ReverseMap();

            CreateMap<FGShopOrderMaterialIssue, FGShopOrderMaterialIssueDto>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssue, FGShopOrderMaterialIssuePostDto>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssue, FGShopOrderMaterialIssueUpdateDto>().ReverseMap();

            CreateMap<FGShopOrderMaterialIssueGeneral, FGShopOrderMaterialIssueGeneralDto>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssueGeneral, FGShopOrderMaterialIssueGeneralPostDto>().ReverseMap();
            CreateMap<FGShopOrderMaterialIssueGeneral, FGShopOrderMaterialIssueGeneralUpdateDto>().ReverseMap();


            CreateMap<MaterialIssue, MaterialIssueDto>().ReverseMap();
            CreateMap<MaterialIssue, MaterialIssuePostDto>().ReverseMap();
            CreateMap<MaterialIssue, MaterialIssueUpdateDto>().ReverseMap();
            CreateMap<MaterialIssueItem, MaterialIssueItemDto>().ReverseMap();
            CreateMap<MaterialIssueItem, MaterialIssueItemPostDto>().ReverseMap();
            CreateMap<MaterialIssueItem, MaterialIssueItemUpdateDto>().ReverseMap();

            CreateMap<MaterialReturnNote, MaterialReturnNoteDto>().ReverseMap();
            CreateMap<MaterialReturnNote, MaterialReturnNotePostDto>().ReverseMap();
            CreateMap<MaterialReturnNote, MaterialReturnNoteUpdateDto>().ReverseMap();

            CreateMap<MaterialReturnNoteItem, MaterialReturnNoteItemDto>().ReverseMap();
            CreateMap<MaterialReturnNoteItem, MaterialReturnNoteItemPostDto>().ReverseMap();
            CreateMap<MaterialReturnNoteItem, MaterialReturnNoteItemUpdateDto>().ReverseMap();

            CreateMap<MRNWarehouseDetails, MRNWarehouseDetailsDto>().ReverseMap();
            CreateMap<MRNWarehouseDetails, MRNWarehouseDetailsPostDto>().ReverseMap();
            CreateMap<MRNWarehouseDetails, MRNWarehouseDetailsUpdateDto>().ReverseMap();

            CreateMap<OQC, OQCDto>().ReverseMap();
            CreateMap<OQC, OQCPostDto>().ReverseMap();
            CreateMap<OQC, OQCUpdateDto>().ReverseMap();

            CreateMap<MaterialRequests, MaterialRequestsDto>().ReverseMap();
            CreateMap<MaterialRequests, MaterialRequestsPostDto>().ReverseMap();
            CreateMap<MaterialRequests, MaterialRequestUpdateDto>().ReverseMap();

            CreateMap<MaterialRequestItems, MaterialRequestItemsDto>().ReverseMap();
            CreateMap<MaterialRequestItems, MaterialRequestItemPostDto>().ReverseMap();
            CreateMap<MaterialRequestItems, MaterialRequestItemUpdateDto>().ReverseMap();

            CreateMap<MRStockDetails, MRStockDetailsDto>().ReverseMap();
            CreateMap<MRStockDetails, MRStockDetailsPostDto>().ReverseMap();
            CreateMap<MRStockDetails, MRStockDetailsUpdateDto>().ReverseMap();

            CreateMap<MaterialRequestItems, UpdateInventoryBalanceQty>().ReverseMap();
            CreateMap<MRStockDetails, UpdateInventoryBalanceQty>().ReverseMap();

            CreateMap<MaterialReturnNoteItem, MRNUpdateInventoryBalanceQty>().ReverseMap();
            CreateMap<MRNWarehouseDetails, MRNUpdateInventoryBalanceQty>().ReverseMap();







        }
    }
}