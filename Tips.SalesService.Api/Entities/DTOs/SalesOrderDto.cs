using Entities.Enums;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.Enum;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public string? QuoteRef { get; set; }

        [Precision(13, 1)]
        public int? QuoteRevisionNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public OrderStatus? SOStatus { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }

        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public string? BillTo { get; set; }
        public int BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int ShipToId { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }

        public decimal? Total { get; set; }
        public string? ReasonForModification { get; set; }

        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }

        [Precision(13, 3)]
        public decimal? TotalAmountWithInstallationCharges { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }
        [DefaultValue(false)]
        public bool ConfirmStatus { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool SoConfirmationStatus { get; set; }
        [DefaultValue(false)]
        public bool ApproveStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<SalesOrderItemsDto>? SalesOrdersItems { get; set; }
        public List<SalesOrderAdditionalChargesDto>? SalesOrderAdditionalCharges { get; set; }
    }
    public class SalesOrderforKeusDto
    {
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }

        [Precision(13, 1)]
        public int? QuoteRevisionNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

        [Precision(13, 1)]
        public int? RevisionNumber { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }

        public OrderStatus SOStatus { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }

        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }


        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int? ShipToId { get; set; }
        public string? PaymentTerms { get; set; }

        [Precision(18, 3)]
        public decimal? Total { get; set; }

        public string? Remarks { get; set; }
        public string Unit { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }
        public string? ReasonForModification { get; set; }
        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }

        [Precision(13, 3)]
        public decimal? TotalAmountWithInstallationCharges { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }
        [DefaultValue(false)]
        public bool ConfirmStatus { get; set; }
        [DefaultValue(false)]
        public bool ApproveStatus { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool SoConfirmationStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? FirstEmailSalesNo { get; set; }
        public DateTime? FirstEmailSentOn { get; set; }
        public decimal? FirstEmailSalesRevNo { get; set; }
        public decimal? FirstEmailSalesValue { get; set; }
        public decimal? SalesUntaxedValue { get; set; }

    }
    public class SalesOrderPostDto
    {
        public string? LeadId { get; set; }
        public string? SalesPerson { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }

        [Precision(13, 1)]
        public int? QuoteRevisionNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }

        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int? ShipToId { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Remarks { get; set; }
        public decimal? Total { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }
        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }

        [Precision(13, 3)]
        public decimal? TotalAmountWithInstallationCharges { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        public DateTime? ConfirmDate { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }
        public List<SalesOrderItemsPostDto>? SalesOrderItemsPostDtos { get; set; }
        public List<SalesOrderAdditionalChargesPostDto>? SalesOrderAdditionalChargesPostDtos { get; set; }
    }

    public class SalesOrderUpdateDto
    {
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? SalesPerson { get; set; }
        public string ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }

        [Precision(13, 1)]
        public int? QuoteRevisionNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }

        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int? ShipToId { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Remarks { get; set; }
        public decimal? Total { get; set; }
        public string? ReasonForModification { get; set; }
        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }

        [Precision(13, 3)]
        public decimal? TotalAmountWithInstallationCharges { get; set; }

        [Precision(18, 3)]
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }
        public OrderStatus SOStatus { get; set; }
        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }

        [Required]
        public string Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SalesOrderItemsUpdateDto>? SalesOrderItemsUpdateDtos { get; set; }
        public List<SalesOrderAdditionalChargesUpdateDto>? SalesOrderAdditionalChargesUpdateDtos { get; set; }
    }

    public class ListofSalesOrderDetails
    {
        public int SalesOrderId { get; set; }
        public string? PONumber { get; set; }
        public string? SalesOrderNumber { get; set; }

    }
    public class SalesOrderIdNameListDto
    {
        public int Id { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? PONumber { get; set; }
    }

    public class SalesOrderSearchDto
    {
        public List<string>? ProjectNumber { get; set; }
        public List<string>? SalesOrderNumber { get; set; }
        public List<string>? CustomerName { get; set; }
        public List<OrderStatus>? SOStatus { get; set; }
    }
    public class SalesOrderReportDto
    {
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public int? QuoteRevisionNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public int? RevisionNumber { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }

        public OrderStatus SOStatus { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }

        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }


        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int? ShipToId { get; set; }
        public string? PaymentTerms { get; set; }

        [Precision(18, 3)]
        public decimal? Total { get; set; }

        public string? Remarks { get; set; }
        public string Unit { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }
        public string? ReasonForModification { get; set; }
        [Precision(13, 3)]
        public decimal? InstallationCharges { get; set; }
        public decimal? TotalAmountWithInstallationCharges { get; set; }
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }
        public decimal? SpecialDiscountAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal TotalFinalAmount { get; set; }
        public bool ConfirmStatus { get; set; }
        public bool ApproveStatus { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool SoConfirmationStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SalesOrderItemsReportDto>? SalesOrdersItems { get; set; }
        public List<SalesOrderAdditionalChargesDto>? SalesOrderAdditionalCharges { get; set; }
    }

    public class CoverageReportChildItemReqQtyDto
    {

        public string? ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal? TotalRequiredQty { get; set; }
    }
    public class CoverageReportChildItemReqQtyDataDto
    {
        public string ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal RequiredQty { get; set; }
        public string UOM { get; set; }

    }
    public class CoverageReportChildItemReqQtyDataByProjectNoDto
    {
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public PartType PartType { get; set; }
        public decimal RequiredQty { get; set; }

    }

    public class SalesOrderSPResportDTO
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
    }
    public class SOSummarySPResportDTO
    {
        public string? CustomerId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
    }
    public class SOMonthlyConsumptionDto
    {
        public string? CustomerId { get; set; }
    }
    public class CustomerWiseTransactionSPReport
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? ReportType { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? MaterialGroup { get; set; }
        public decimal? SOOrderQty { get; set; }
        public decimal? DoQty { get; set; }
        public decimal? RDoQty { get; set; }
        public decimal? InvoicedQty { get; set; }
        public decimal? ReturnInvQty { get; set; }
        public decimal? RODOQTY { get; set; }
        public decimal? ODOQTY { get; set; }
        public decimal? OpenGrinQty { get; set; }
    }

    public class RfqSalesOrderSPResportDTO
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
    }
    public class RfqSalesOrderSPResportDTOForTrans
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class ForecastSalesOrderSPResportDTO
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
    }
    public class ForecastSalesOrderSPResportDTOForTrans
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class SalesOrderFGItemNumberDto
    {
        public string? FGItemNumber { get; set; }
    }
    public class SalesOrderEmailPostDto
    {
        public string SentTo { get; set; }
        public string? CusEmail { get; set; }
        public string jasperfileUrl { get; set; }
        public int SalesOrderid { get; set; }
        public string WhatsAppPhoneNos { get; set; }
    }
    public class SOEmailMessageSuccessMessage
    {
        public string SalesOrderNumber { get; set; }
        public int RevisionNumber { get; set; }
    }

    public class SalesOrderId_SP
    {
        public int Id { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public int? RevisionNumber { get; set; }
        public OrderStatus SOStatus { get; set; }
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int? ShipToId { get; set; }
        public string? PaymentTerms { get; set; }
        public decimal? Total { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? ReasonForModification { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }
        public int? QuoteRevisionNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public decimal? InstallationCharges { get; set; }
        public decimal? TotalAmountWithInstallationCharges { get; set; }
        public decimal? TotalAdditionalCharges { get; set; }
        public string? SpecialDiscountType { get; set; }
        public decimal? SpecialDiscountAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal TotalFinalAmount { get; set; }
        public string? LeadId { get; set; }
        public bool ConfirmStatus { get; set; }
        public bool ApproveStatus { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool SoConfirmationStatus { get; set; }
        public string? SalesPerson { get; set; }
        public string? QuoteRef { get; set; }
        public string? SalesOrdersItems { get; set; }
        public string? SalesOrderAdditionalCharges { get; set; }
    }
}
