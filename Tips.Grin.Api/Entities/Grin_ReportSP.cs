using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Entities
{
    public class Grin_ReportSP
    {
        public string? GrinNumber { get; set; }
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorAddress { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? PONumber { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? ItemDescription { get; set; }
        public string? ManufactureBatchNumber { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal? Qty { get; set; }

        [Precision(13, 3)]
        public decimal? AcceptedQty { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public decimal? totalvalue { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? Remarks { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOC { get; set; }
        public string? TallyVoucher { get; set; }
    }
}