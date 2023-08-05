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

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public OrderStatus? SOStatus { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }

        //PO Details
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        //Billing&Shipping
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
        [DefaultValue(false)]
        public bool ApproveStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<SalesOrderItemsDto>? SalesOrdersItems { get; set; }
        public List<SalesOrderAdditionalChargesDto>? SalesOrderAdditionalCharges { get; set; }
    }

    public class SalesOrderPostDto
    {
        public string? LeadId { get; set; }
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

        //PO Details
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        //Billing&Shipping
        public string? BillTo { get; set; }
        public int? BillToId { get; set; }
        public string? ShipTo { get; set; }
        public int? ShipToId { get; set; }
        public string? PaymentTerms { get; set; }        
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

        //PO Details
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        //Billing&Shipping 
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

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }
        [Precision(18, 3)]
        public decimal? TotalAmount { get; set; }
        [Precision(18, 3)]
        public decimal TotalFinalAmount { get; set; }

        [Required]
        public string Unit { get; set; }

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
        //public string? ProjectNumber { get; set; }
        //public string? SalesOrderNumber { get; set; }
        //public string? CustomerName { get; set; }
        //public OrderStatus? SOStatus { get; set; }
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

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        //aravind
        public OrderStatus SOStatus { get; set; }
        public SalesOrderStatus SalesOrderStatus { get; set; }

        //PO Details
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        //Billing&Shipping
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
        [DefaultValue(false)]
        public bool ApproveStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SalesOrderItemsDto>? SalesOrdersItems { get; set; }
        public List<SalesOrderAdditionalChargesDto>? SalesOrderAdditionalCharges { get; set; } 
    }
}
