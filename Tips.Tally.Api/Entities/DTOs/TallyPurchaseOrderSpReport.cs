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
            public decimal? ExchangeRate { get; set; }
            public string? Address { get; set; }
            public string? PINCode { get; set; }
            public string? State { get; set; }
            public string? Country { get; set; }

            // JSON: GSTIN_No
            public string ? GSTIN_No { get; set; }

            public string? GST_Type { get; set; }

            // JSON: POData
            public string? POData { get; set; }
        

    }
    public class TallyPurchaseOrderSpReportDto
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

        // JSON: GSTIN_No
        public List<GSTINNoDtos>? GSTIN_No { get; set; }

        public string? GST_Type { get; set; }

        // JSON: POData
        public List<PODataDtos>? POData { get; set; }
    }


    public class GSTINNoDtos
    {
        public string? GSTNNumber { get; set; }
    }

    public class PODataDtos
    {
        public string? UOM { get; set; }
        public decimal? GST_Percentage { get; set; }  // represents "GST%"
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }

        public List<DueDateDtos>? DueDate { get; set; }

        public string? HSNCode { get; set; }
        public string? Discount { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? Location { get; set; }
        public decimal? Quantity { get; set; }

        public List<ProjectCodeDtos>? ProjectCode { get; set; }

        public string? ProjectName { get; set; }
        public string? AccountingLedger { get; set; }
    }
    public class DueDateDtos
    {
        public DateTime? PODeliveryDate { get; set; }
    }

    public class ProjectCodeDtos
    {
        public string? ProjectNumber { get; set; }
    }

}
