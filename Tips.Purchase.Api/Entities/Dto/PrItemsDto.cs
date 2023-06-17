using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? PrNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PartType PartType { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? SpecialInstruction { get; set; }

        public List<PrAddProjectDto>? PrAddprojectsDtoList { get; set; }
        public List<PrAddDeliveryScheduleDto>? PrAddDeliverySchedulesDtoList { get; set; }
    }

    public class PrItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public string? PrNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PartType PartType { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? SpecialInstruction { get; set; }

        public List<PrAddProjectPostDto>? PrAddprojectsDtoPostList { get; set; }
        public List<PrAddDeliverySchedulePostDto>? PrAddDeliverySchedulesDtoPostList { get; set; }
    }

    public class PrItemsUpdateDto
    {
        public string? ItemNumber { get; set; }
        public string? PrNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PartType PartType { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? SpecialInstruction { get; set; }

        public List<PrAddProjectDtoUpdate>? PrAddprojectsDtoUpdateList { get; set; }
        public List<PrAddDeliveryScheduleUpdateDto>? PrAddDeliverySchedulesDtoUpdateList { get; set; }
    }

}
