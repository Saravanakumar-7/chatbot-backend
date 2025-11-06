namespace Tips.Tally.Api.Entities.DTOs
{
    public class TallyPurchaseOrderSpReport
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Number { get; set; }
        public string? PartyAccount { get; set; }
        public decimal? TransactionValue { get; set; }
        public string? Currency { get; set; }
        public string? ExchangeRate { get; set; }
        public string? Address { get; set; }
        public string? PINCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GSTIN_No { get; set; }
        public string? GST_Type { get; set; }
        public string? POItems { get; set; }
    }
    public class TallyPurchaseOrderSpReportDto
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Number { get; set; }
        public string? PartyAccount { get; set; }
        public decimal? TransactionValue { get; set; }
        public string? Currency { get; set; }
        public string? ExchangeRate { get; set; }
        public string? Address { get; set; }
        public string? PINCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GSTIN_No { get; set; }
        public string? GST_Type { get; set; }
        public List<POItemsSpDto> POItems { get; set; }
    }


    public class POItemsSpDto
    {
        public string? GST { get; set; }
        public string? UOM { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string? HSNCode { get; set; }
        public string? Discount { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Location { get; set; }
        public decimal? Quantity { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public string? AccountingLedger { get; set; }
    }

}
