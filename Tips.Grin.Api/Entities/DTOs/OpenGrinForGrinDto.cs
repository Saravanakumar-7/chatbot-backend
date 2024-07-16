namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForGrinDto
    {
        public int Id { get; set; }
        public string? SenderName { get; set; }
        public string? OpenGrinNumber { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public bool IsOpenGrinForGrinCompleted { get; set; }
        public bool IsOpenGrinForIqcCompleted { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinForGrinItemsDto> OpenGrinForGrinItems { get; set; }
    }
    public class OpenGrinForGrinPostDto
    {
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinForGrinItemsPostDto> OpenGrinForGrinItems { get; set; }
    }
    public class OpenGrinForGrinUpdateDto
    {
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinForGrinItemsUpdateDto> OpenGrinForGrinItems { get; set; }
    }
    public class OpenGrinForIQCConfirmationSaveDto
    {
        public string? OpenGrinNumber { get; set; }
        public int OpenGrinForGrinId { get; set; }

        public OpenGrinForIQCConfirmationItemsSaveDto OpenGrinForIQCConfirmationItemsSaveDto { get; set; }

    }
    public class OpenGrinNoForOpenGrinIqcAndBinning
    {
        public string? OpenGrinNumber { get; set; }
        public int OpenGrinForGrinId { get; set; }

    }
}
