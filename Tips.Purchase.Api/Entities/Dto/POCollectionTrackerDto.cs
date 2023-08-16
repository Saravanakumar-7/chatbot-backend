using System.ComponentModel;
using Tips.Purchase.Api.Entities.Dto;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class POCollectionTrackerDto
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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDownDto>? POBreakDown { get; set; }
    }
    public class POCollectionTrackerPostDto
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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDownPostDto>? POBreakDown { get; set; }
    }
    public class POCollectionTrackerUpdateDto
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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDownUpdateDto>? POBreakDown { get; set; }
    }
    public class POCollectionTrackerDetailsDto
    {
        public decimal? TotalSumOfPOAmount { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public List<OpenPurchaseOrderDetailsDto>? OpenPurchaseOrderDetails { get; set; }
    }
    public class OpenPurchaseOrderDetailsDto
    {
        public int PurchaseOrderId { get; set; }
        public string PONumber { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? PendingValue { get; set; }
        public decimal AmountRecieved { get; set; }
    }
    public class POCollectionTrackerSearchDto
    {
        public List<string> VendorId { get; set; }
        public List<string> VendorName { get; set; }
        public List<string>? Remarks { get; set; }
    }
}