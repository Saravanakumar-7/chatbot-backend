namespace Tips.Purchase.Api.Entities
{
    public class PurchaseOrderSPReportForTrans
    {
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? Qty { get; set; }
        public decimal? TotalWithTax { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }
        public decimal? BalanceQty { get; set; }
        public string? PoRequested_Date { get; set; }  
        public string? PoConfirmationDate { get; set; }  
        public decimal? pocQty { get; set; }
        public decimal? confirmationvalue { get; set; }
        public string? PODeliveryDate { get; set; }  
        public decimal? PODeliveryQty { get; set; }
        public decimal? schedulevalue { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
        public decimal? projectvalue { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
