namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqCustomFieldDto
    {
        public int Id { get; set; }
        public string? CustomGroupName { get; set; }
        public string? LabelName { get; set; }
        public string? Type { get; set; }
        public string? MaxLength { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqCustomFieldDtoPost
    {
        public string? CustomGroupName { get; set; }
        public string? LabelName { get; set; }
        public string? Type { get; set; }
        public string? MaxLength { get; set; }
    }
    public class RfqCustomFieldDtoUpdate
    {
        public int Id { get; set; }
        public string? CustomGroupName { get; set; }
        public string? LabelName { get; set; }
        public string? Type { get; set; }
        public string? MaxLength { get; set; }
        public string? Unit { get; set; }
       
    }
}
