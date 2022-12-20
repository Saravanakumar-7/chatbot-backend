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
        public string UOM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoAddProjectDto> PoAddprojectsDtoList { get; set; }
        public List<PoAddDeliveryScheduleDto> PoAddDeliverySchedulesDtoList { get; set; }

    }

    public class PoItemsDtoPost
    {
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoAddProjectDtoPost> PoAddprojectsDtoPostList { get; set; }
        public List<PoAddDeliveryScheduleDtoPost> PoAddDeliverySchedulesDtoPostList { get; set; }

    }

    public class PoItemsDtoUpdate
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public string SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PoAddProjectDtoUpdate> PoAddprojectsDtoUpdateList { get; set; }
        public List<PoAddDeliveryScheduleDtoUpdate> PoAddDeliverySchedulesDtoUpdateList { get; set; }

    }
    public class PurchaseOrderItemNoListDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
    }

    public class PoItemListDto
    {
        public int Id { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal PoUnitPrice { get; set; }
        public decimal PoOrderedQuantity { get; set; }
        public decimal PoBalancedQuantity { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
    }
}