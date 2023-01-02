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
        public decimal Qty { get; set; }
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

        public List<PoAddProjectDto> POAddprojectsDtoList { get; set; }
        public List<PoAddDeliveryScheduleDto> POAddDeliverySchedulesDtoList { get; set; }

    }

    public class PoItemsDtoPost
    {
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
        public string SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }


        public List<PoAddProjectDtoPost> POAddprojectsDtoPostList { get; set; }
        public List<PoAddDeliveryScheduleDtoPost> POAddDeliverySchedulesDtoPostList { get; set; }

    }

    public class PoItemsDtoUpdate
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
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

        public List<PoAddProjectDtoUpdate> POAddprojectsDtoUpdateList { get; set; }
        public List<PoAddDeliveryScheduleDtoUpdate> POAddDeliverySchedulesDtoUpdateList { get; set; }

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
        public decimal POUnitPrice { get; set; }
        public decimal POOrderedQty { get; set; }
        public decimal POBalancedQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal Total { get; set; }
    }
}