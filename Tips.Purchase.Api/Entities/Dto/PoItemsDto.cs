using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PoItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public string? SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        [Precision(13, 3)]
        public decimal SGST { get; set; }
        [Precision(13, 3)]
        public decimal CGST { get; set; }
        [Precision(13, 3)]
        public decimal IGST { get; set; }
        [Precision(13, 3)]
        public decimal UTGST { get; set; }
        [Precision(13, 3)]
        public decimal Total { get; set; }
   
        public List<PoAddProjectDto>? POAddprojectsDtoList { get; set; }
        public List<PoAddDeliveryScheduleDto>? POAddDeliverySchedulesDtoList { get; set; }

    }

    public class PoItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public string? SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        [Precision(13, 3)]
        public decimal SGST { get; set; }
        [Precision(13, 3)]
        public decimal CGST { get; set; }
        [Precision(13, 3)]
        public decimal IGST { get; set; }
        [Precision(13, 3)]
        public decimal UTGST { get; set; }
        [Precision(13, 3)]
        public decimal Total { get; set; }

        public List<PoAddProjectPostDto>? POAddprojectsDtoPostList { get; set; }
        public List<PoAddDeliverySchedulePostDto>? POAddDeliverySchedulesDtoPostList { get; set; }

    }

    public class PoItemsUpdateDto
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public string? SpecialInstruction { get; set; }
        public bool IsTechnicalDocsRequired { get; set; }
        [Precision(13, 3)]
        public decimal SGST { get; set; }
        [Precision(13, 3)]
        public decimal CGST { get; set; }
        [Precision(13, 3)]
        public decimal IGST { get; set; }
        [Precision(13, 3)]
        public decimal UTGST { get; set; }
        [Precision(13, 3)]
        public decimal Total { get; set; }

        public List<PoAddProjectUpdateDto>? POAddprojectsDtoUpdateList { get; set; }
        public List<PoAddDeliveryScheduleUpdateDto>? POAddDeliverySchedulesDtoUpdateList { get; set; }

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
        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal POOrderedQty { get; set; }
        [Precision(13, 3)]
        public decimal POBalancedQty { get; set; }
        [Precision(13, 3)]
        public decimal SGST { get; set; }
        [Precision(13, 3)]
        public decimal CGST { get; set; }
        [Precision(13, 3)]
        public decimal IGST { get; set; }
        [Precision(13, 3)]
        public decimal UTGST { get; set; }
        [Precision(13, 3)]
        public decimal Total { get; set; }
    }
}