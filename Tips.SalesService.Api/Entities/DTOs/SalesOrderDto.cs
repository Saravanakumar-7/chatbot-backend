using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Tips.SalesService.Api.Entities.Dto;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public OrderStatus SOStatus { get; set; } = 0;

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


        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<SalesOrderItemsDto>? SalesOrdersItems { get; set; }
    }

    public class SalesOrderPostDto
    {
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

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


        public List<SalesOrderItemsPostDto>? SalesOrderItemsPostDtos { get; set; }
    }

    public class SalesOrderUpdateDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

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


        [Required]
        public string Unit { get; set; }

        public List<SalesOrderItemsUpdateDto>? SalesOrderItemsUpdateDtos { get; set; }
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
    }

    public class SalesOrderSearchDto
    {
        public List<string>? ProjectNumber { get; set; }
        public List<string>? SalesOrderNumber { get; set; }
        public List<string>? CustomerName { get; set; }
        public List<OrderStatus>? SOStatus { get; set; }
    }
}
