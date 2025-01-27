namespace Tips.Warehouse.Api.Entities
{
    public class ConsumptionSPReport
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? DoNumber {  get; set; }
        public string? FGItemNumber { get; set; }
        public string? DoLotNumber {  get; set; }
        public string? WorkOrderNumber { get; set; }
        public decimal? WorkOrderQty { get; set; }
        public decimal? InvoiceQty { get; set; }
        public decimal? BalanceToinvoiceQty { get; set; }
        public string? Partnumber { get; set; }
        public string? TransactionFrom { get; set; }
        public string? MftrPartnumber { get; set; }
        public string? PoLotNumber { get; set; }
        public decimal? CusumedQty { get; set; }
        public decimal? QtyInInvoice { get; set; }
        public decimal? QtyInFG { get; set; }
        public DateTime Materialissue { get; set; }
        public string? GrinNumber { get; set; }
        public DateTime GrinDate { get; set; }
        public string? Vendor { get; set; }
        public string? PoNumber { get; set; }
        public string? BOENo { get; set; }
        public decimal? GrinQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Tax { get; set; }
        public decimal? OtherCosts { get; set; }
        public decimal? UOM { get; set; }
        public decimal? UOC { get; set; }
    }
}
