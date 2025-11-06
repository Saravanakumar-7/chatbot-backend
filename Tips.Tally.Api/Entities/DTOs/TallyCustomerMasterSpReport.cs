namespace Tips.Tally.Api.Entities.DTOs
{
    public class TallyCustomerMasterSpReport
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Group { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? GSTINNo { get; set; }
        public string? GSTType { get; set; }
        public string? Currency { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class TallyCustomerMasterSpReportDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Group { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }

        public List<GSTINNoDto> GSTINNo { get; set; }
        public string? GSTType { get; set; }
        public string? Currency { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class GSTINNoDto
    {
        public string GSTNNumber { get; set; }
    }
}
