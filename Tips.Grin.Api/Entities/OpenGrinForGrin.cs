namespace Tips.Grin.Api.Entities
{
    public class OpenGrinForGrin
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
        public List<OpenGrinForGrinItems> OpenGrinForGrinItems { get; set; }
    }
}
