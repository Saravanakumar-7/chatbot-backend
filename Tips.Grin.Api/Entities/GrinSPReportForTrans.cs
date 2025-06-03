using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class GrinSPReportForTrans
    {
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string? GrinNumber { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? GateEntryNo { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorAddress { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? MPN { get; set; }
        public string? ManufactureBatchNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Qty { get; set; }
        public decimal? ProjectAcceptedQty { get; set; }
        public string? LotNumber { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? totalvalue { get; set; }
        public string? Remarks { get; set; }
        public bool? TallyStatus { get; set; }
        public string? BENumber { get; set; }
        public decimal? GrinAcceptedQty { get; set; } 
    }
}
