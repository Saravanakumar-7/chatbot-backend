using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PoPartType PartType { get; set; }
        public string? SpecialInstruction { get; set; }
        public List<PRItemsDocumentUploadDto>? PRItemFiles { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public PrStatus PrStatus { get; set; }
       // public List<PRItemsDocumentUploadDto>? Upload { get; set; }
        public List<PrAddProjectDto>? PrAddprojectsDtoList { get; set; }
        public List<PrAddDeliveryScheduleDto>? PrAddDeliverySchedulesDtoList { get; set; }
        public List<PrSpecialInstructionDto>? prSpecialInstructionsDtoList { get; set; }
    }

    public class PrItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PoPartType PartType { get; set; }
        public string? PRFileIds { get; set; }

        public string? SpecialInstruction { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }
       // public List<PRItemsDocumentUploadPostDto>? Upload { get; set; } 

        public List<PrAddProjectPostDto>? PrAddprojectsDtoPostList { get; set; }
        public List<PrAddDeliverySchedulePostDto>? PrAddDeliverySchedulesDtoPostList { get; set; }
        public List<PrSpecialInstructionPostDto>? prSpecialInstructionsPostList { get; set; }
    }

    public class PrItemsUpdateDto
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PoPartType PartType { get; set; }
        public string? SpecialInstruction { get; set; }
        public string? PRFileIds { get; set; }
        // public List<PRItemsDocumentUploadUpdateDto>? Upload { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public List<PrAddProjectDtoUpdate>? PrAddprojectsDtoUpdateList { get; set; }
        public List<PrAddDeliveryScheduleUpdateDto>? PrAddDeliverySchedulesDtoUpdateList { get; set; }
        public List<PrSpecialInstructionUpdateDto>? prSpecialInstructionsUpdateList { get; set; }
    }
    public class PrItemsReportDto
    {
        public int Id { get; set; }
        public string? PrNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PoPartType PartType { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }

        public List<PrAddProjectReportDto>? prAddprojectsDtoList { get; set; }
        public List<PrAddDeliveryScheduleReportDto>? prAddDeliverySchedulesDtoList { get; set; }
        public List<PrSpecialInstructionDto>? prSpecialInstructionsDtoList { get; set; }
    }

}
