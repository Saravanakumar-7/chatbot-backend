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
        public decimal? UOM { get; set; }
        [Precision(13, 3)]
        public decimal? Quantity { get; set; }
        public string? SpecialInstruction { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
 
        public List<PrAddProjectDto> prAddprojectsDto { get; set; }
        public List<PrAddDeliveryScheduleDto> prAddDeliverySchedulesDto { get; set; }
    }

    public class PrItemsDtoPost
    {
        public string? ItemNumber { get; set; }

        public string? MftrItemNumber { get; set; }

        public string? Description { get; set; }

        public decimal? UOM { get; set; }
        [Precision(13, 3)]
        public decimal? Quantity { get; set; }

        public string? SpecialInstruction { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrAddProjectDtoPost> prAddprojectsDtoPost { get; set; }
        public List<PrAddDeliveryScheduleDtoPost> prAddDeliverySchedulesDtoPost { get; set; }
    }

    public class PrItemsDtoUpdate
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }

        public string? MftrItemNumber { get; set; }

        public string? Description { get; set; }

        public decimal? UOM { get; set; }
        [Precision(13, 3)]
        public decimal? Quantity { get; set; }

        public string? SpecialInstruction { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrAddProjectDtoUpdate> prAddprojectsDtoUpdate { get; set; }
        public List<PrAddDeliveryScheduleDtoUpdate> prAddDeliverySchedulesDtoUpdate { get; set; }
    }

}
