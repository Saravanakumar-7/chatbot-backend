namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrAddProjectDto
    {
        public int Id { get; set; }
        public string PrProjectNumber { get; set; }
        public decimal PrProjectQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
       
    }
    public class PrAddProjectDtoPost
    {
        public string PrProjectNumber { get; set; }
        public decimal PrProjectQty { get; set; }
     

    }

    public class PrAddProjectDtoUpdate
    {
        
        public string PrProjectNumber { get; set; }
        public decimal PrProjectQty { get; set; }
        public string? CreatedBy { get; set; }

        public string Unit { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}
