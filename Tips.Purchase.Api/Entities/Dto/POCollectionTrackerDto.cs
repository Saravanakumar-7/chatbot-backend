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
        public string? Unit { get; set; }
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
        public string? Unit { get; set; }
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
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<POBreakDownUpdateDto>? POBreakDown { get; set; }
    }
    public class POCollectionTrackerDetailsDto
    {
        public decimal TotalSumOfPOAmount { get; set; }
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
    public class PayableSPReportWithParamDTO
    {
        public string? PONumber { get; set; }
        public string? VendorName { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class PayableSPReport
    {
        public string? PONumber { get; set; } // TEXT -> string?
        public DateTime? POCreationDate { get; set; } // DATETIME -> DateTime?
        public DateTime? PODeliveryDate { get; set; } // DATETIME -> DateTime?
        public string? VendorNumber { get; set; } // VARCHAR -> string?
        public string? VendorName { get; set; } // TEXT -> string?
        public string? ProjectNumber { get; set; } // TEXT -> string?
        public string? ItemNumber { get; set; } // TEXT -> string?
        public string? Description { get; set; } // TEXT -> string?
        public decimal? Qty { get; set; } // DECIMAL -> decimal?
        public string? UOC { get; set; } // TEXT -> string?
        public decimal? UnitPrice { get; set; } // DECIMAL -> decimal?
        public decimal? SumOfAmount { get; set; } // DECIMAL -> decimal?
        public string? GrinNumber { get; set; } // TEXT -> string?
        public decimal? GrinQty { get; set; } // DECIMAL -> decimal?
        public string? IQCNumber { get; set; } // TEXT -> string?
        public decimal? IqcQty { get; set; } // DECIMAL -> decimal?
        public decimal? POValue { get; set; } // DECIMAL -> decimal?
        public decimal? TotalPaidSumofAmount { get; set; } // DECIMAL -> decimal?
        public decimal? TotalBalancesumofamounttobePaid { get; set; } // DECIMAL -> decimal?
    }

}