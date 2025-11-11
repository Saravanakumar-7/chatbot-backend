namespace Tips.Tally.Api.Entities.DTOs
{
    public class TallySalesOrderSpReport
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Number { get; set; }
        public string? PartyAccount { get; set; }
        public decimal? TransactionValue { get; set; }
        public string? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string? Address { get; set; }
        public string? PINCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GSTIN_No { get; set; }
        public string? GST_Type { get; set; }
        public string? SalesOrderItems { get; set; }
    }
    public class TallySalesOrderSpReportDto
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Number { get; set; }
        public string? PartyAccount { get; set; }
        public decimal? TransactionValue { get; set; }
        public string? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string? Address { get; set; }
        public string? PINCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GSTIN_No { get; set; }
        public string? GST_Type { get; set; }
        public List<SalesOrderItemDtos> SalesOrderItems { get; set; }
    }

    public class SalesOrderItemDtos
    {
        public string? UOM { get; set; }

        // Renamed "GST%" → "GSTPercent" for valid C# naming
        public string? GSTPercent { get; set; }

        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string? HSNCode { get; set; }
        public string? Discount { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Location { get; set; }
        public decimal? Quantity { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectNumber { get; set; }
        public string? AccountingLedger { get; set; }
    }
}
