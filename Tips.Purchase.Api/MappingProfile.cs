using AutoMapper;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PurchaseOrderSPReport, PurchaseOrderSPReportDTO>().ReverseMap();

            CreateMap<PurchaseOrder, PurchaseOrderDto>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderPostDto>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderUpdateDto>().ReverseMap();

            CreateMap<PoItem, PoItemsDto>().ReverseMap();
            CreateMap<PoItem, PoItemsPostDto>().ReverseMap();
            CreateMap<PoItem, PoItemsUpdateDto>().ReverseMap();

            CreateMap<PoAddProject, PoAddProjectDto>().ReverseMap();
            CreateMap<PoAddProject, PoAddProjectPostDto>().ReverseMap();
            CreateMap<PoAddProject, PoAddProjectUpdateDto>().ReverseMap();

            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleDto>().ReverseMap();
            CreateMap<PoAddDeliverySchedule, PoAddDeliverySchedulePostDto>().ReverseMap();
            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleUpdateDto>().ReverseMap();

       

            CreateMap<PurchaseRequisition, PurchaseRequisitionDto>().ReverseMap();
            CreateMap<PurchaseRequisition, PurchaseRequisitionPostDto>().ReverseMap();
            CreateMap<PurchaseRequisition, PurchaseRequisitionUpdateDto>().ReverseMap();

            CreateMap<PrItem, PrItemsDto>().ReverseMap();
            CreateMap<PrItem, PrItemsPostDto>().ReverseMap();
            CreateMap<PrItem, PrItemsUpdateDto>().ReverseMap();

            CreateMap<PrAddProject, PrAddProjectDto>().ReverseMap();
            CreateMap<PrAddProject, PrAddProjectPostDto>().ReverseMap();
            CreateMap<PrAddProject, PrAddProjectDtoUpdate>().ReverseMap();

            CreateMap<PrAddDeliverySchedule, PrAddDeliveryScheduleDto>().ReverseMap();
            CreateMap<PrAddDeliverySchedule, PrAddDeliverySchedulePostDto>().ReverseMap();
            CreateMap<PrAddDeliverySchedule, PrAddDeliveryScheduleUpdateDto>().ReverseMap();

            CreateMap<DocumentUpload, DocumentUploadDto>().ReverseMap();
            CreateMap<DocumentUpload, DocumentUploadPostDto>().ReverseMap();
            CreateMap<DocumentUpload, DocumentUploadUpdateDto>().ReverseMap();
            CreateMap<DocumentUpload, GetDownloadUrlDto>().ReverseMap();
            CreateMap<DocumentUpload, GetPRDownloadUrlDto>().ReverseMap();


            CreateMap<PurchaseOrderDto, DocumentUploadDto>().ReverseMap();
             

            CreateMap<PoItem, PurchaseOrderUpdateQtyDetailsDto>().ReverseMap();

            CreateMap<PrSpecialInstruction, PrSpecialInstructionDto>().ReverseMap();
            CreateMap<PrSpecialInstruction, PrSpecialInstructionPostDto>().ReverseMap();
            CreateMap<PrSpecialInstruction, PrSpecialInstructionUpdateDto>().ReverseMap();

            CreateMap<PoSpecialInstruction, PoSpecialInstructionDto>().ReverseMap();
            CreateMap<PoSpecialInstruction, PoSpecialInstructionPostDto>().ReverseMap();
            CreateMap<PoSpecialInstruction, PoSpecialInstructionUpdateDto>().ReverseMap();

            CreateMap<POCollectionTracker, POCollectionTrackerDto>().ReverseMap();
            CreateMap<POCollectionTracker, POCollectionTrackerPostDto>().ReverseMap();
            CreateMap<POCollectionTracker, POCollectionTrackerUpdateDto>().ReverseMap();

            CreateMap<POBreakDown, POBreakDownDto>().ReverseMap();
            CreateMap<POBreakDown, POBreakDownPostDto>().ReverseMap();
            CreateMap<POBreakDown, POBreakDownUpdateDto>().ReverseMap();

            CreateMap<PrDetails, PrDetailsDto>().ReverseMap();
            CreateMap<PrDetails, PrDetailsPostDto>().ReverseMap();
            CreateMap<PrDetails, PrDetailsUpdateDto>().ReverseMap();

            CreateMap<PoIncoTerm, PoIncoTermDto>().ReverseMap();
            CreateMap<PoIncoTerm, PoIncoTermPostDto>().ReverseMap();
            CreateMap<PoIncoTerm, PoIncoTermUpdateDto>().ReverseMap();

            CreateMap<PoConfirmationDate, PoConfirmationDateDto>().ReverseMap();
            CreateMap<PoConfirmationDate, PoConfirmationDatePostDto>().ReverseMap();
            CreateMap<PoConfirmationDate, PoConfirmationDateUpdateDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, PRItemsDocumentUploadDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, PRItemsDocumentUploadPostDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, PRItemsDocumentUploadUpdateDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, PRItemsGetDownloadUrlDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, GetPRItemsDownloadUrlDto>().ReverseMap();

        }
    }
}
