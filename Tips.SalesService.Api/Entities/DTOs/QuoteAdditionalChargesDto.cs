namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteAdditionalChargesDto
    {
        public int Id { get; set; }
        public string? AdditionalChargesLabelName { get; set; }
        public decimal AddtionalChargesValueType { get; set; }
        public decimal AddtionalChargesValueAmount { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class QuoteAdditionalChargesDtoPost
    {
        public string? AdditionalChargesLabelName { get; set; }
        public decimal AddtionalChargesValueType { get; set; }
        public decimal AddtionalChargesValueAmount { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class QuoteAdditionalChargesDtoUpdate
    {
        public int Id { get; set; }
        public string? AdditionalChargesLabelName { get; set; }
        public decimal AddtionalChargesValueType { get; set; }
        public decimal AddtionalChargesValueAmount { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
