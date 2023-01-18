namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteAdditionalChargesDto
    {
        public int Id { get; set; }
        public string? AdditionalChargesLabelName { get; set; }
        public string AddtionalChargesValueType { get; set; }
        public decimal AddtionalChargesValueAmount { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; } 
    }
    public class QuoteAdditionalChargesDtoPost
    {
        public string? AdditionalChargesLabelName { get; set; }
        public string AddtionalChargesValueType { get; set; }
        public decimal AddtionalChargesValueAmount { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; } 
    }
    public class QuoteAdditionalChargesDtoUpdate
    {
         public string? AdditionalChargesLabelName { get; set; }
        public string AddtionalChargesValueType { get; set; }
        public decimal AddtionalChargesValueAmount { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; } 
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
