using Microsoft.Build.Framework;
using System.ComponentModel;
using Tips.SalesService.Api.Entities.Dto;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string QuoteNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string RevNo { get; set; }

        //PO Details
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public DateTime ReceivedDate { get; set; }

        //Billing&Shipping
        public string BillTo { get; set; }
        public int BillToId { get; set; }
        public string ShipTo { get; set; }
        public int ShipToId { get; set; }
        public string PaymentTerms { get; set; }
        public string Remarks { get; set; }
        public string Unit { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<SalesOrderItemsDto>? SalesOrderItems { get; set; }
    }

    public class SalesOrderDtoPost
    {
        public string ProjectNumber { get; set; }
        public string QuoteNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string RevNo { get; set; }

        //PO Details
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public DateTime ReceivedDate { get; set; }

        //Billing&Shipping
        public string BillTo { get; set; }
        public int BillToId { get; set; }
        public string ShipTo { get; set; }
        public int ShipToId { get; set; }
        public string PaymentTerms { get; set; }
        public string Remarks { get; set; }      

        public List<SalesOrderItemsDtoPost>? SalesOrderItems { get; set; }
    }

    public class SalesOrderDtoUpdate
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string QuoteNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string RevNo { get; set; }

        //PO Details
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public DateTime ReceivedDate { get; set; }

        //Billing&Shipping
        public string BillTo { get; set; }
        public int BillToId { get; set; }
        public string ShipTo { get; set; }
        public int ShipToId { get; set; }
        public string PaymentTerms { get; set; }
        public string Remarks { get; set; }

        [Required]
        public string Unit { get; set; }

        public List<SalesOrderItemsDtoUpdate>? SalesOrderItems { get; set; }
    }
   


}
