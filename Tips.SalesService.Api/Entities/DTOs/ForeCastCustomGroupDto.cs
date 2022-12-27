namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ForeCastCustomGroupDto
    {
        public int Id { get; set; }
        public string CustomGroupName { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForeCastCustomGroupDtoPost
    {
        public string CustomGroupName { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForeCastCustomGroupDtoUpdate
    {
        public int Id { get; set; }
        public string CustomGroupName { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
