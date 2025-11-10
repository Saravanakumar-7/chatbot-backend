using Newtonsoft.Json;

namespace Tips.Tally.Api.Entities.DTOs
{
    public class TallybtodeliveryorderSpReport
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

        // JSON string from DB
        public string? SalesOrderItems { get; set; }
    }

    public class TallybtodeliveryorderSpReportDto
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

        public List<SalesOrderItemDto>? SalesOrderItems { get; set; }
    }

    // Level 1: Each sales order item
    public class SalesOrderItemDto
    {
        public string? UOM { get; set; }

        [JsonProperty("GST%")]
        public string? GSTPercent { get; set; }

        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string? HSNCode { get; set; }
        public decimal? Discount { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Location { get; set; }
        public decimal? Quantity { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }

        // List of child sales orders
        public List<SalesOrderItemDetailDto>? SalesOrderItems { get; set; }

        public string? AccountingLedger { get; set; }
    }

    // Level 2: Child SOID + DueDate list
    public class SalesOrderItemDetailDto
    {
        public int? SOID { get; set; }
        public List<DueDateDto>? DueDate { get; set; }
    }

    // Level 3: Actual date object
    public class DueDateDto
    {
        public DateTime? Date { get; set; }
    }
}
