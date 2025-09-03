using Entities;
//using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PoItemsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? Description { get; set; }
        public string? PONumber { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        public decimal? Allowance { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal? ShortClosedQty { get; set; }
        public PoPartType? PartType { get; set; }
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
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal TotalWithTax { get; set; }
        public PoStatus PoStatus { get; set; }
        public string? ReasonforShortClose { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? Remarks { get; set; }
        public string? drawingRevNo { get; set; }
        public string? POApprovalIRemarks { get; set; }
        public string? POApprovalIIRemarks { get; set; }
        public string? POApprovalIIIRemarks { get; set; }
        public string? POApprovalIVRemarks { get; set; }
        public List<PoAddProjectDto>? POAddprojects { get; set; }
        public List<PoAddDeliveryScheduleDto>? POAddDeliverySchedules { get; set; }
        public List<PoConfirmationDateDto>? POConfirmationDates { get; set; }
        public List<PoSpecialInstructionDto>? POSpecialInstructions { get; set; }
        public List<PrDetailsDto>? PrDetails { get; set; }

    }

    public class PoItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public PoPartType? PartType { get; set; }
        public decimal? Allowance { get; set; }
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
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal TotalWithTax { get; set; }
        public string? ReasonforShortClose { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? Remarks { get; set; }
        public string? drawingRevNo { get; set; }
        public List<PoAddProjectPostDto>? POAddprojects { get; set; }
        public List<PoAddDeliverySchedulePostDto>? POAddDeliverySchedules { get; set; }
       // public List<PoConfirmationDatePostDto>? POConfirmationDates { get; set; }
        public List<PoSpecialInstructionPostDto>? POSpecialInstructions { get; set; }
        public List<PrDetailsPostDto>? PrDetails { get; set; }

    }

    public class PoItemsUpdateDto
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PoPartType? PartType { get; set; }
        public decimal? Allowance { get; set; }
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
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal TotalWithTax { get; set; }
        public bool NowShortClosed { get; set; }
        public string? ReasonforShortClose { get; set; }
        public string? ShortClosedBy { get; set; }
        public PoStatus PoStatus { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? Remarks { get; set; }
        public string? drawingRevNo { get; set; }
        public string? POApprovalIRemarks { get; set; }
        public string? POApprovalIIRemarks { get; set; }
        public string? POApprovalIIIRemarks { get; set; }
        public string? POApprovalIVRemarks { get; set; }
        public List<PoAddProjectUpdateDto>? POAddprojects { get; set; }
        public List<PoAddDeliveryScheduleUpdateDto>? POAddDeliverySchedules { get; set; }
        public List<PoConfirmationDateUpdateDto>? POConfirmationDates { get; set; }
        public List<PoSpecialInstructionUpdateDto>? POSpecialInstructions { get; set; }
        public List<PrDetailsUpdateDto>? PrDetails { get; set; }

    }
    public class PoItemsApprovalUpdateDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public decimal? KitRevisionNo { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public PoPartType? PartType { get; set; }
        public decimal? Allowance { get; set; }
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
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal TotalWithTax { get; set; }
        public bool NowShortClosed { get; set; }
        public string? ReasonforShortClose { get; set; }

        public string? ShortClosedBy { get; set; }
        public PoStatus PoStatus { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? Remarks { get; set; }
        public string? drawingRevNo { get; set; }
        public string? POApprovalIRemarks { get; set; }
        public string? POApprovalIIRemarks { get; set; }
        public string? POApprovalIIIRemarks { get; set; }
        public string? POApprovalIVRemarks { get; set; }
        public List<PoAddProjectApprovalUpdateDto>? POAddprojects { get; set; }
        public List<PoAddDeliveryScheduleApprovalUpdateDto>? POAddDeliverySchedules { get; set; }
        public List<PoConfirmationDateApprovalUpdateDto>? POConfirmationDates { get; set; }
        public List<PoSpecialInstructionApprovalUpdateDto>? POSpecialInstructions { get; set; }
        public List<PrDetailsApprovalUpdateDto>? PrDetails { get; set; }

    }

    public class PoItemsShortCloseDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? PONumber { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal? ShortClosedQty { get; set; }
        public PoPartType? PartType { get; set; }
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
        public decimal? Allowance { get; set; }
        [Precision(13, 3)]
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal TotalWithTax { get; set; }
        public PoStatus PoStatus { get; set; }
        public bool NowShortClosed {  get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? ReasonforShortClose { get; set; }
        public string? Remarks { get; set; }
        public string? drawingRevNo { get; set; }
        public List<PoAddProjectShortCloseDto>? POAddprojects { get; set; }
        public List<PoAddDeliveryScheduleShortCloseDto>? POAddDeliverySchedules { get; set; }
        public List<PoConfirmationDateShortCloseDto>? POConfirmationDates { get; set; }
        public List<PoSpecialInstructionShortCloseDto>? POSpecialInstructions { get; set; }
        public List<PrDetailsShortCloseDto>? PrDetails { get; set; }

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
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal Total { get; set; }
    }

    public class PurchaseOrderUpdateQtyDetailsDto
    {

        public string? ItemNumber { get; set; }
        public decimal Qty { get; set; }
        public string? PONumber { get; set; }

    }
    public class PurchaseOrderStatusUpdateDto
    {

        public string? ItemNumber { get; set; }
        public decimal Qty { get; set; }
        public string? PONumber { get; set; }
        public int PoItemId { get; set; }

    }
    public class OpenPurchaseOrderDto
    {
        public string? ItemNumber { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal Qty { get; set; }
        public string? PONumber { get; set; }

    }

    public class OpenPurchaseOrderByProjectNoDto
    {
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal Qty { get; set; }
        public string? PONumber { get; set; }

    }
    public class PoItemsReportDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? Description { get; set; }
        public string? PONumber { get; set; }

        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public PoPartType? PartType { get; set; }
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
        public decimal? SubTotal { get; set; }
        [Precision(13, 3)]
        public decimal TotalWithTax { get; set; }

        public List<PoAddProjectReportDto>? POAddprojects { get; set; }
        public List<PoAddDeliveryScheduleReportDto>? POAddDeliverySchedules { get; set; }
        public List<PoConfirmationDateReportDto>? POConfirmationDates { get; set; }
        public List<PoSpecialInstructionDto>? POSpecialInstructions { get; set; }
        public List<PrDetailsDto>? PrDetails { get; set; }

    }


    public class OpenPoQuantityDto
    {

        public string? ItemNumber { get; set; }
        public decimal OpenPoQty { get; set; }

    }
    public class coveragePOByMultipleProjectDto
    {
        public List<string> itemNumberList { get; set; }
        public List<string> projectNo { get; set; }

    }
    public class PoItemConfirmationDateDto
    {
        public int PoId { get; set; }
        public int POItemDetailId { get; set; }
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [DefaultValue(false)]
        public bool IsInitialConfirmationDateDone { get; set; }
    }
}