using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal? Qty { get; set; }
        public string? SpecialInstruction { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
 
        public List<PrAddProjectDto> PrAddprojectsDtoList { get; set; }
        public List<PrAddDeliveryScheduleDto> PrAddDeliverySchedulesDtoList { get; set; }
    }

    public class PrItemsDtoPost
    {
        public string? ItemNumber { get; set; }

        public string? MftrItemNumber { get; set; }

        public string? Description { get; set; }

        public string? UOM { get; set; }
        public decimal? Qty { get; set; }

        public string? SpecialInstruction { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrAddProjectDtoPost> PrAddprojectsDtoPostList { get; set; }
        public List<PrAddDeliveryScheduleDtoPost> PrAddDeliverySchedulesDtoPostList { get; set; }
    }

    public class PrItemsDtoUpdate
    {

        public string? ItemNumber { get; set; }

        public string? MftrItemNumber { get; set; }

        public string? Description { get; set; }

        public string? UOM { get; set; }
      
        public decimal? Qty { get; set; }

        public string? SpecialInstruction { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrAddProjectDtoUpdate> PrAddprojectsDtoUpdateList { get; set; }
        public List<PrAddDeliveryScheduleDtoUpdate> PrAddDeliverySchedulesDtoUpdateList { get; set; }
    }

}
