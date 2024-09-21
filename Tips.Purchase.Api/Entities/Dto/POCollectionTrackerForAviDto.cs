using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class POCollectionTrackerForAviDto
    {
        public int Id { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string? Remarks { get; set; }
        public decimal TotalSumOfPOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public decimal AlreadyRecievedWithPercentage { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDownForAviDto>? POBreakDownForAvi { get; set; }
    }
    public class POCollectionTrackerForAviPostDto
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string? Remarks { get; set; }
        public decimal TotalSumOfPOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public decimal AlreadyRecievedWithPercentage { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDownForAviPostDto>? POBreakDownForAvi { get; set; }
    }
    public class POCollectionTrackerForAviUpdateDto
    {
        public int Id { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string? Remarks { get; set; }
        public decimal TotalSumOfPOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public decimal AlreadyRecievedWithPercentage { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDownForAviUpdateDto>? POBreakDownForAvi { get; set; }
    }
    public class POCollectionTrackerForAviDetailsDto
    {
        public decimal TotalSumOfPOAmount { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public List<OpenPurchaseOrderForAviDetailsDto>? OpenPurchaseOrderForAviDetails { get; set; }
    }
    public class OpenPurchaseOrderForAviDetailsDto
    {
        public int PurchaseOrderId { get; set; }
        public string PONumber { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? PendingValue { get; set; }
        public decimal AmountRecieved { get; set; }
    }
    public class POCollectionTrackerForAviSearchDto
    {
        public List<string> VendorId { get; set; }
        public List<string> VendorName { get; set; }
        public List<string>? Remarks { get; set; }
    }
    public class PO_GRIN_IQC_POBreakDownDetailsDto
    { 
        
    }
}
