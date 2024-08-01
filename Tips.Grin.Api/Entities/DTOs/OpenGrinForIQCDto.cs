namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForIQCDto
    {
        public int Id { get; set; }
        public string? OpenGrinForIQCNumber { get; set; }
        public string? OpenGrinNumber { get; set; }
        public int OpenGrinForGrinId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public bool IsOpenGrinForIqcCompleted { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinForIQCItemsDto>? OpenGrinForIQCItems { get; set; }
    }
    public class OpenGrinForIQCPostDto
    {
        public string? OpenGrinNumber { get; set; }
        public int OpenGrinForGrinId { get; set; }
        public List<OpenGrinForIQCItemsPostDto>? OpenGrinForIQCItems { get; set; }
    }
    public class OpenGrinForIQCUpdateDto
    {
        public int Id { get; set; }
        public string? OpenGrinNumber { get; set; }
        public int OpenGrinForGrinId { get; set; }
        public string Unit { get; set; }
        public List<OpenGrinForIQCItemsUpdateDto>? OpenGrinForIQCItems { get; set; }
    }
    public class OpenGrinForIQCSaveDto
    {
        public string? OpenGrinNumber { get; set; }
        public int OpenGrinForGrinId { get; set; }
        public OpenGrinForIQCItemsSaveDto? OpenGrinForIQCItems { get; set; }
    }
    public class OpenGrinForIQCSPReportWithParamDto
    {
        public string? OpenGrinForIQCNumber { get; set; }
        public string? ItemNumber { get; set; }
    }
    public class OpenGrinForIQCSPReport
    {
        // Fields from opengrinforiqcs
        public string? OpenGrinForIQCNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? IqcDate { get; set; }

        // Fields from opengrinforiqcitems
        public string? ItemNumber { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? RejectedQty { get; set; }
        public string? RemarksIQCItems { get; set; }

        // Fields from opengrinforgrins
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiptRefNo { get; set; }

        // Fields from opengrinforgrinprojectnumbers
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
    }
}
