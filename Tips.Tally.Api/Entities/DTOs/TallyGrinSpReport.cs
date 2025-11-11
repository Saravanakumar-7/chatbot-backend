namespace Tips.Tally.Api.Entities.DTOs
{
    public class TallyGrinSpReport
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Number { get; set; }
        public string? PartyAccount { get; set; }
        public string? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string? Address { get; set; }
        public string? PINCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GSTIN_No { get; set; }
        public string? GST_Type { get; set; }
        public string? GRINParts { get; set; }
    }
    public class TallyGrinSpReportDto
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Number { get; set; }
        public string? PartyAccount { get; set; }
        public string? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string? Address { get; set; }
        public string? PINCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GSTIN_No { get; set; }
        public string? GST_Type { get; set; }
        public List<GRINPartDto> GRINParts { get; set; }
    }
    public class GRINPartDto
    {
        public string? UOM { get; set; }

        // Property name changed for C# validity — cannot use "%".
        public string? GSTPercent { get; set; }

        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public List<PODataDto>? POData { get; set; }
        public string? HSNCode { get; set; }
        public string? Discount { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Location { get; set; }
        public string? ProjectName { get; set; }
        public string? AccountingLedger { get; set; }
    }

    public class ProjectCodeDto
    {
        public string? ProjectNumber { get; set; }
    }

    public class PODataDto
    {
        public int? PID { get; set; }
        public List<DueDateDto>? DueDate { get; set; }
        public List<ProjectCodeDto>? ProjectCode { get; set; }
    }
}
