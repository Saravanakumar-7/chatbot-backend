using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PoItemsDto
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public decimal UOM { get; set; }
        [Precision(13, 8)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool WantToAttachedTheTechnicalDoc { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoAddProjectDto> poAddprojectsDto { get; set; }
        public List<PoAddDeliveryScheduleDto> poAddDeliverySchedulesDto { get; set; }
    
    }

    public class PoItemsDtoPost
    {
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public decimal UOM { get; set; }
        [Precision(13, 8)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool WantToAttachedTheTechnicalDoc { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoAddProjectDtoPost> poAddprojectsDtoPost { get; set; }
        public List<PoAddDeliveryScheduleDtoPost> poAddDeliverySchedulesDtoPost { get; set; }
       
    }

    public class PoItemsDtoUpdate
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public decimal UOM { get; set; }
        [Precision(13, 8)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool WantToAttachedTheTechnicalDoc { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoAddProjectDtoUpdate> poAddprojectsDtoUpdate { get; set; }
        public List<PoAddDeliveryScheduleDtoUpdate> poAddDeliverySchedulesDtoUpdate { get; set; }
        
    }

}
