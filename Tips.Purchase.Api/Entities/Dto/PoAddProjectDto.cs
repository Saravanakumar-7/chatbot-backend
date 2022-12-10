namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PoAddProjectDto
    {
        public int Id { get; set; }
        public string PoProjectNumber { get; set; }
        public decimal PoProjectQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class PoAddProjectDtoPost
    {
        
        public string PoProjectNumber { get; set; }
        public decimal PoProjectQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class PoAddProjectDtoUpdate
    {
        public int Id { get; set; }
        public string PoProjectNumber { get; set; }
        public decimal PoProjectQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
