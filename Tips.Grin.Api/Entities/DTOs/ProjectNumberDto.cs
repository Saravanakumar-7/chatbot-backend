namespace Tips.Grin.Api.Entities.DTOs
{
    public class ProjectNumberDto
    {
        public int Id { get; set; }
        public string? Projectnumber { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ProjectQuantity { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class ProjectNumberDtoPost
    {
        public string? Projectnumber { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ProjectQuantity { get; set; }
    }

    public class ProjectNumberDtoUpdate
    {
        public int Id { get; set; }
        public string? Projectnumber { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ProjectQuantity { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
