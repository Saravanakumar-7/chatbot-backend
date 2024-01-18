using Entities;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Entities
{
    public class Tras_PO_ConfirmationDate
    {
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? PartNumber { get; set; }
        public decimal? Qty { get; set; }
        public decimal? TotalWithTax { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }
        public decimal? BalanceQty { get; set; }
        public DateTime? PoRequested_Date { get; set; }
        public DateTime? PoConfirmationDate { get; set; }
        public decimal? pocQty { get; set; }
        public decimal? confirmationvalue { get; set; }
        public DateTime? PODeliveryDate { get; set; }
        public decimal? PODeliveryQty { get; set; }
        public decimal? schedulevalue { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
        public decimal? projectvalue { get; set; }
    }
}

