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
            CreateMap<Tras_POSPReport, Tras_POSPReportDTO>().ReverseMap();
            
            CreateMap<PurchaseOrder, PONameList>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderDto>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderPostDto>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderUpdateDto>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderIdNameListDto>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderForShortCloseDto>().ReverseMap();

            CreateMap<PoItem, PoItemsDto>().ReverseMap();
            CreateMap<PoItem, PoItemsPostDto>().ReverseMap();
            CreateMap<PoItem, PoItemsUpdateDto>().ReverseMap();
            CreateMap<PoItem, PoItemsShortCloseDto>().ReverseMap();

            CreateMap<PoAddProject, PoAddProjectDto>().ReverseMap();
            CreateMap<PoAddProject, PoAddProjectPostDto>().ReverseMap();
            CreateMap<PoAddProject, PoAddProjectUpdateDto>().ReverseMap();
            CreateMap<PoAddProject, PoAddProjectShortCloseDto>().ReverseMap();

            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleDto>().ReverseMap();
            CreateMap<PoAddDeliverySchedule, PoAddDeliverySchedulePostDto>().ReverseMap();
            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleUpdateDto>().ReverseMap();
            CreateMap<PoAddDeliverySchedule, PoAddDeliveryScheduleShortCloseDto>().ReverseMap();

            CreateMap<PurchaseRequisition, PurchaseRequisitionDto>().ReverseMap();
            CreateMap<PurchaseRequisition, PurchaseRequisitionPostDto>().ReverseMap();
            CreateMap<PurchaseRequisition, PurchaseRequisitionUpdateDto>().ReverseMap();
            CreateMap<PurchaseRequisition, PurchaseRequisitionIdNameListDto>().ReverseMap();

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
            CreateMap<PoSpecialInstruction, PoSpecialInstructionShortCloseDto>().ReverseMap();

            CreateMap<POCollectionTracker, POCollectionTrackerDto>().ReverseMap();
            CreateMap<POCollectionTracker, POCollectionTrackerPostDto>().ReverseMap();
            CreateMap<POCollectionTracker, POCollectionTrackerUpdateDto>().ReverseMap();

            CreateMap<POCollectionTrackerForAvi, POCollectionTrackerForAviDto>().ReverseMap();
            CreateMap<POCollectionTrackerForAvi, POCollectionTrackerForAviPostDto>().ReverseMap();
            CreateMap<POCollectionTrackerForAvi, POCollectionTrackerForAviUpdateDto>().ReverseMap();

            CreateMap<POBreakDown, POBreakDownDto>().ReverseMap();
            CreateMap<POBreakDown, POBreakDownPostDto>().ReverseMap();
            CreateMap<POBreakDown, POBreakDownUpdateDto>().ReverseMap();

            CreateMap<POBreakDownForAvi, POBreakDownForAviDto>().ReverseMap();
            CreateMap<POBreakDownForAvi, POBreakDownForAviPostDto>().ReverseMap();
            CreateMap<POBreakDownForAvi, POBreakDownForAviUpdateDto>().ReverseMap();

            CreateMap<PrDetails, PrDetailsDto>().ReverseMap();
            CreateMap<PrDetails, PrDetailsPostDto>().ReverseMap();
            CreateMap<PrDetails, PrDetailsUpdateDto>().ReverseMap();
            CreateMap<PrDetails, PrDetailsShortCloseDto>().ReverseMap();

            CreateMap<PoIncoTerm, PoIncoTermDto>().ReverseMap();
            CreateMap<PoIncoTerm, PoIncoTermPostDto>().ReverseMap();
            CreateMap<PoIncoTerm, PoIncoTermUpdateDto>().ReverseMap();
            CreateMap<PoIncoTerm, PoIncoTermShortCloseDto>().ReverseMap();

            CreateMap<PoConfirmationDate, PoConfirmationDateDto>().ReverseMap();
            CreateMap<PoConfirmationDate, PoConfirmationDatePostDto>().ReverseMap();
            CreateMap<PoConfirmationDate, PoConfirmationDateUpdateDto>().ReverseMap();
            CreateMap<PoConfirmationDate, PoConfirmationDateShortCloseDto>().ReverseMap();

            CreateMap<PRItemsDocumentUpload, PRItemsDocumentUploadDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, PRItemsDocumentUploadPostDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, PRItemsDocumentUploadUpdateDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, PRItemsGetDownloadUrlDto>().ReverseMap();
            CreateMap<PRItemsDocumentUpload, GetPRItemsDownloadUrlDto>().ReverseMap();


            CreateMap<PurchaseOrderAdditionalCharges, PurchaseOrderAdditionalChargesDto>().ReverseMap();
            CreateMap<PurchaseOrderAdditionalCharges, PurchaseOrderAdditionalChargesPostDto>().ReverseMap();
            CreateMap<PurchaseOrderAdditionalCharges, PurchaseOrderAdditionalChargesUpdateDto>().ReverseMap();
            CreateMap<PurchaseOrderAdditionalCharges, PurchaseOrderShortCloseAdditionalChargesDto>().ReverseMap();

        }
    }
}
