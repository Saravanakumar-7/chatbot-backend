namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteGeneralDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? PriceList { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedUnitPrice { get; set; }
        public string SpecialDiscountType { get; set; }
        public decimal SpecialDiscountAmount { get; set; }
        public decimal TotalDiscountedUnitPrice { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal HSNNo { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class QuoteGeneralDtoPost
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? PriceList { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedUnitPrice { get; set; }
        public string SpecialDiscountType { get; set; }
        public decimal SpecialDiscountAmount { get; set; }
        public decimal TotalDiscountedUnitPrice { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal HSNNo { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class QuoteGeneralDtoUpdate
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? PriceList { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedUnitPrice { get; set; }
        public string SpecialDiscountType { get; set; }
        public decimal SpecialDiscountAmount { get; set; }
        public decimal TotalDiscountedUnitPrice { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal HSNNo { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public decimal SGST { get; set; }
        public decimal Total { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
