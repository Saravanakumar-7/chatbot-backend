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

        [DefaultValue(false)]
        public bool IsDODone { get; set; }

        [DefaultValue(false)]
        public bool NowShortClosed { get; set; }
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
        public string? AdvanceType { get; set; }

        [Precision(18, 3)]
        public decimal? AdvanceAmount { get; set; }
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
        [DefaultValue(false)]
        public bool IsDODone { get; set; }
        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }
        [DefaultValue(false)]
        public bool NowShortClosed { get; set; }
        public string? AdvanceType { get; set; }

        [Precision(18, 3)]
        public decimal? AdvanceAmount { get; set; }
        [Required]
        public string Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SalesOrderItemsUpdateDto>? SalesOrderItemsUpdateDtos { get; set; }
        public List<SalesOrderAdditionalChargesUpdateDto>? SalesOrderAdditionalChargesUpdateDtos { get; set; }
    }

    public class SalesOrderShortCloseDto
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
        [DefaultValue(false)]
        public bool IsDODone { get; set; }
        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }
        [DefaultValue(false)]
        public bool NowShortClosed { get; set; }
        public string? AdvanceType { get; set; }

        [Precision(18, 3)]
        public decimal? AdvanceAmount { get; set; }
        [Required]
        public string Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SalesOrderItemsShortCloseDto>? SalesOrderItemsUpdateDtos { get; set; }
        public List<SalesOrderAdditionalChargesShortCloseDto>? SalesOrderAdditionalChargesUpdateDtos { get; set; }
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
        public string Version { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public PartType PartType { get; set; }
        public decimal RequiredQty { get; set; }

    }
    public class CoverageReportSAChildItemReqQtyDataByProjectNoDto
    {
        public string ItemNumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public PartType PartType { get; set; }
        public decimal RequiredQty { get; set; }
        public decimal ODOQty { get; set; }

    }
    public class SalesOrderSPReportForTransDTO
    {
        public string? ProjectNumber {  get; set; }
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? SOStatus {  get; set; }
        public string? KPN { get; set; }
    }

    public class SalesOrderSPReportDTO
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? CustomerId { get; set; }
    }
    public class SOSummarySPReportDTO
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
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
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

    public class RfqSalesOrderSPReportDTO
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
        public string? CustomerId { get; set; }
    }
    public class RfqSalesOrderSPReportDTOForTrans
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class ForecastSalesOrderSPReportDTO
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
        public string? CustomerId { get; set; }
    }
    public class ForecastSalesOrderSPReportDTOForTrans
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
       // public string jasperfileUrl { get; set; }
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
    public class SOHistoryRevNoListDto
    {
        public int Id { get; set; }
        public int? RevisionNumber { get; set; }

    }
    public class SOHisDetailsDto
    {
        public int Id { get; set; }
        public decimal? ShortClosedQty { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal DispatchQty { get; set; }

    }
    public class SalesOrderDashboardSPReport
    {
        public string? SORange { get; set; }
        public int? NoOfCount { get; set; }
        public decimal? SOValue { get; set; }

    }
    public class SalesOrderDashboardSPReport_Details
    {
        public string Title { get; set; }
        public List<SalesOrderDashboardSPReport> Items { get; set; }
    }   

    public class FinancialYearDashboardSPReport
    {
        public string? FinancialYear { get; set; }
        public int? NoOfCount { get; set; }
        public decimal? Value { get; set; }

    }
    public class FinancialYearDashboardSPReport_Details
    {
        public string Title { get; set; }
        public List<FinancialYearDashboardSPReport> Items { get; set; }
    }
    public class TransactionDashboardSPReport
    {
        public string? ServiceGrin { get; set; }
        public int? NoOfCount { get; set; }
        public decimal? ServiceValue { get; set; }

    }
    public class TransactionDashboardSPReport_bucketId1
    {
        public string? SORange { get; set; }
        public int? NoOfCount { get; set; }
        public decimal? SOValue { get; set; }
    }
    public class TransactionDashboardSPReport_bucketId2
    {
        public string? PORange { get; set; }
        public int? NoOfCount { get; set; }
        public decimal? POValue { get; set; }
    }
    public class TransactionDashboardSPReport_bucketId3
    {
        public string? POGrin { get; set; }
        public int? NoOfCount { get; set; }
        public decimal? GRINValue { get; set; }
    } 
    public class TransactionDashboardSPReport_bucketId5
    {
        public string? OpenGrin { get; set; }
        public int? NoOfCount { get; set; }
    }
    public class TransactionDashboardSPReport_Details
    {
        public string Title { get; set; }
        public List<TransactionDashboardSPReport_bucketId1>? Items1 { get; set; }
        public List<TransactionDashboardSPReport_bucketId2>? Items2 { get; set; }
        public List<TransactionDashboardSPReport_bucketId3>? Items3 { get; set; }
        public List<TransactionDashboardSPReport>? Items4 { get; set; }
        public List<TransactionDashboardSPReport_bucketId5>? Items5 { get; set; }
    }
    public class ReceivableReportsForMultiCustomerIdDto
    {
        public string? CustomerId { get; set; }
    }
    public class ReceivableSPReportsDto
    {
        public string? CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    public class FGSalesOrderSPReport
    {
        public string? SalesOrderNumber { get; set; } 
        public string? ProjectNumber { get; set; } 
        public decimal? ProjectOrderQty { get; set; } 
        public string? CustomerName { get; set; } 
        public string? City { get; set; } 
        public DateTime ProjectYear { get; set; }
    }
    public class FGSalesOrderSPReportWithDate
    {
            public string? SalesOrderNumber { get; set; }
            public string? Description { get; set; }
            public string? ProjectNumber { get; set; }
            public decimal? UnitPrice { get; set; }
            public decimal? ProjectOrderQty { get; set; }
            public string? CustomerName { get; set; }
            public DateTime? PODate { get; set; }
            public DateTime? ScheduleDate { get; set; }
            public string? POMinusSchedulDate { get; set; }
            public DateTime? DODate { get; set; }
            public string? DodateMinusScheduleDate { get; set; }
            public string? Unit { get; set; }
    }

    public class FGSalesOrderSPReportDto
    {
        public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class RecievableDayWiseSPReportDto
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? PaymentMode { get; set; }
        public string? PaymentRefNo { get; set; }
        public decimal? TotalSumOfSOAmount { get; set; }
        public decimal? AlreadyRecieved { get; set; }
        public decimal? AmountRecieved { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? salesordernumber { get; set; }
        public string? producttype { get; set; }
        public string? typeofsolution { get; set; }
        public string? SalesPerson { get; set; }
        
    }

    public class AdvanceRecievableSPReportDto
    {
        public string? CustomerId { get; set; } 
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; } 
        public decimal? TotalSumOfSOAmount { get; set; } 
        public decimal? AlreadyRecieved { get; set; } 
        public decimal? AmountRecieved { get; set; } 
        public decimal? TotalAdvance { get; set; } 
        public string? TypeOfSolution { get; set; } 
        public string? ProductType { get; set; } 
    }


    public class SalesOrderDetailsTOSDto
    {
        public string? SalesOrderNumber { get; set; }
        public decimal? RevisionNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public int? QuoteRevisionNumber { get; set; }
    }
    public class SOLeadWiseDataSPReportDTO
    {
        public string? SOFirstSalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
    public class SOLeadWiseDataSPReport
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? TypeOfSolution { get; set; }
        public DateTime? SOFirstCreatedOn { get; set; }
        public string? SOFirstSalesOrderNumber { get; set; }
        public decimal? SOFirstValue { get; set; }
        public string? SOFirstDiscount { get; set; }
        public string? SOFirstDiscountType { get; set; }
        public double? FirstSODiscountValue { get; set; }
        public DateTime? SOLatestCreatedOn { get; set; }
        public string? SOLatestSalesOrderNumber { get; set; }
        public decimal? SOLatestValue { get; set; }
        public string? SOLatestDiscount { get; set; }
        public string? SOLatestDiscountType { get; set; }
        public double? LatestSODiscountValue { get; set; }
    }
   
    public class FQToFSSPReport
    {
        public string? LeadId { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? FirstQuoteCreatedNumber { get; set; }
        public string? FirstQuoteRevisionNumber { get; set; }
        public DateTime? FirstQuoteCreatedOn { get; set; }

        public string? FirstQuoteKPN { get; set; }
        public string? FirstQuoteKPNDescription { get; set; }
        public decimal? FirstQuoteQty { get; set; }
        public decimal? FirstQuoteDiscountAmount { get; set; }
        public decimal? FirstQuoteTotalFinalAmount { get; set; }

        public string? FirstQuoteSentNumber { get; set; }
        public string? FirstQuoteSentRevisionnumber { get; set; }
        public DateTime? FirstQuoteSentCreatedOn { get; set; }

        public string? FirstQuoteSentKPN { get; set; }
        public string? FirstQuoteSentKPNDescription { get; set; }
        public decimal? FirstQuoteSentQty { get; set; }
        public decimal? FirstQuoteSentDiscountAmount { get; set; }
        public decimal? FirstQuoteSentTotalFinalAmount { get; set; }

        public string? LatestSalesOrderSentNumber { get; set; }
        public DateTime? LatestSOSentCreatedOn { get; set; }
        public int? LatestSOSentRevisionnumber { get; set; }
        public string? LatestSOSentKPN { get; set; }
        public string? LatestSOSentKPNDescription { get; set; }

        public int? StatusEnum { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public string? LatestSOSentDiscount { get; set; }
        public decimal? LatestSOSentTaxedValue { get; set; }
    }

    public class FQToFSSPReportDto
    {
        public string? FirstQuoteSentNumber { get; set; }
        public string? SOLatestSalesOrderSentNumber { get; set; }
        public string? LeadId { get; set; }
        public string? TypeOfSolution { get; set; }
    }

    public class FQToFSFirstSOSPReportDto
    {
        public string? Leadid { get; set; }
    }

    public class FQToFSFirstSOSPReport
    {
        public string? LeadId { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? FirstSOSalesOrderNumber { get; set; }
        public DateTime? FirstSOCreatedOn { get; set; }
        public int? FirstSORevisionnumber { get; set; }
        public string? FirstSOKPN { get; set; }
        public string? FirstSOKPNDescription { get; set; }
        public string? FirstSOStatusEnum { get; set; }
        public decimal? FirstSOOrderQty { get; set; }
        public decimal? FirstSOBalanceQty { get; set; }
        public decimal? FirstSODispatchQty { get; set; }
        public string? FirstSODiscount { get; set; }
        public decimal? FirstSOTaxedValue { get; set; }
        public string? FirstSOQuoteNumber { get; set; }
    }

    public class FQToFSFirstQuoteSPReport
    {
        public string? LeadId { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? FirstQuoteCreatedNumber { get; set; }
        public decimal? FirstQuoteRevisionNumber { get; set; }
        public DateTime? FirstQuoteCreatedOn { get; set; }
        public decimal? FirstQuotetotalamount { get; set; }
        public string? FirstQuoteKPN { get; set; }
        public string? FirstQuoteKPNDescription { get; set; }
        public decimal? FirstQuoteQty { get; set; }
        public decimal? FirstQuoteDiscountAmount { get; set; }
        public decimal? FirstQuoteTotalFinalAmount { get; set; }
    }
    public class FQToFSFirstQuoteSentSPReport
    {
    public string? LeadId { get; set; }
    public string? TypeOfSolution { get; set; }
    public string? FirstQuoteSentNumber { get; set; }
    public decimal? FirstQuoteSentRevisionnumber { get; set; }
    public DateTime? FirstQuoteSentCreatedOn { get; set; }
    public string? FirstQuoteSentKPN { get; set; }
    public string? FirstQuoteSentKPNDescription { get; set; }
    public decimal? FirstQuoteSentQty { get; set; }
    public decimal? FirstQuoteSentDiscountAmount { get; set; }
    public decimal? FirstQuoteSentTotalFinalAmount { get; set; }
}

    public class FQToFSLatestSOSPReport
    {
        public string? LeadId { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? LatestSalesOrderSentNumber { get; set; }
        public DateTime? LatestSOSentCreatedOn { get; set; }
        public int? LatestSOSentRevisionnumber { get; set; }
        public string? LatestSOSentKPN { get; set; }
        public string? LatestSOSentKPNDescription { get; set; }
        public int? LatestSOSentStatusEnum { get; set; }
        public decimal? LatestSOSentOrderQty { get; set; }
        public decimal? LatestSOBalanceQty { get; set; }
        public decimal? LatestSODispatchQty { get; set; }
        public string? LatestSODiscount { get; set; }
        public decimal? LatestSOTaxedValue { get; set; }
        public string? LatestSOQuoteNumber { get; set; }
    }

    public class SalesOrderSPReportForTrans
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? LeadId { get; set; }
        public string? OrderType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? MaterialGroup { get; set; }
        public string? ItemType { get; set; }
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? PoCreatedon { get; set; }
        public string? SalesPerson { get; set; }
        public DateTime? sodate { get; set; }
        public int? SOStatus { get; set; }
        public string? KPN { get; set; }
        public string? KPNDescription { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public string? PriceList { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? BasicAmount { get; set; }
        public string? DiscountType { get; set; }
        public string? Discount { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? itempricelist { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public DateTime? scheduledate { get; set; }
        public decimal? scheduleqnty { get; set; }


    }
    public class InitialAdvanceCustomerSPReport
    {
        public int? Id { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal? TotalSumOfSOAmount { get; set; }
        public decimal? AmountRecieved { get; set; }
        public decimal? AlreadyRecieved { get; set; }
        public string? PaymentMode { get; set; }
        public string? PaymentRefNo { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? Remarks { get; set; }
        public decimal? AlreadyRecievedWithPercentage { get; set; }
    }

    public class InitialAdvanceCustomerSPReportDto
    {
        public string? CustomerId { get; set; }
    }
    public class SalesRevNoSPReportParamDTO
    {
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
    }
    public class SalesRevNoSPReportParam
    {
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? SalesOrderNumber { get; set; }
        public int? RevisionNumber { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public DateTime? created_date { get; set; }
        public string? CreatedBy { get; set; }
        public string? OrderType { get; set; }
        public string? KPN { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? OrderQty { get; set; }
        public string? Discount { get; set; }
        public double? DiscountValue { get; set; }
        public double? NetQuote_PostDiscount { get; set; }
        public decimal? TotalAmountWithInstallationCharges { get; set; }
        public decimal? TotalFinalAmount { get; set; }
    }


}
