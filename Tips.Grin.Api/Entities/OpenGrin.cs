namespace Tips.Grin.Api.Entities
{
    public class OpenGrin
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinParts> OpenGrinParts { get; set; }
    }
}
