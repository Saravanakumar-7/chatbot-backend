using Entities.Enums;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ConsumptionSPReportDto
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? DoNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? DoLotNumber { get; set; }
        public string? WorkOrderNumber { get; set; }
        public decimal? WorkOrderWipQty { get; set; }
        public decimal? WorkOrderQty { get; set; }
        public decimal? InvoiceQty { get; set; }
        //public decimal? BalanceToinvoiceQty { get; set; }
        public string? PartNumber { get; set; }
        public decimal? CusumedQty { get; set; }
        public string? TransactionFrom { get; set; }
        public string? MftrPartnumber { get; set; }
        public string? PPLotNumber { get; set; }
        //public decimal? QtyInInvoice { get; set; }
        //public decimal? QtyInFG { get; set; }
        public DateTime? MaterialissueDate { get; set; }
        public string? GrinNumber { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? Vendor { get; set; }
        public string? PoNumber { get; set; }
        public decimal? BOENo { get; set; }
        public decimal? GrinQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Tax { get; set; }
        public decimal? OtherCosts { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }
    }

    public class ShopOrderComsumpDto
    {
        public string? ShopOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
        public decimal ReleaseQty {  get; set; }
        public decimal WipQty { get; set; }
    }
    public class GrinComsumpDto
    {
        public string? GrinNumber { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? VendorName { get; set; }
        public string? PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string? PONumber { get; set; }
        public decimal BOENo { get; set; }
        public decimal GrinQty { get; set; }
        public decimal GrinUnitPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal OtherCosts { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }

    }
    public class ShopOrderComsumpDetailsDto
    {
        public string? ShopOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal InvoicedQty { get; set; }
        public string? BTONumber { get; set; }
    }

    public class InvoiceBTODetailsDto
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? DONumber { get; set; }
        public string? FGItemNumber { get; set; }
        public decimal InvoicedQty { get; set; }
        public string SalesOrderNumber { get; set; }
        public string? LotNumber { get; set; } // This is the ShopOrderNumber
    }

    public class InvoiceBTOShopOrderDetailsDto
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? DONumber { get; set; }
        public string? FGItemNumber { get; set; }
        public decimal InvoicedQty { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? LotNumber { get; set; } // This is the ShopOrderNumber
        // Shop order quantities from ShopOrderComsumpDto
        public decimal ReleaseQty { get; set; }
        public decimal WipQty { get; set; }
    }
    public class EnggChildBomComsumpDetailsDto
    {
        public string? ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal Quantity { get; set; }

    }
}
