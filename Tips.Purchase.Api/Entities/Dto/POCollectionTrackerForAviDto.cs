using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tips.Purchase.Api.Entities.DTOs;
using Entities.Enums;

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
        public PurchaseOrder PurchaseOrder { get; set; }
        public List<Grin>? Grins { get; set; }
        public List<Iqc>? Iqcs { get; set; }
        public List<GrinsForServiceItems>? grinsForServiceItems { get; set; }
        public List<IQCForServiceItems>? iqcsForServiceItems { get; set; }
        public List<POCollectionTrackerForAvi> POCollectionTrackers { get; set; }
    }
    public class GrinandIqcDetail
    {
        public Data Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
    }

    public class Data
    {
        public List<Grin> Grins { get; set; }
        public List<Iqc>? Iqcs { get; set; }
    }

    public class Grin
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }

        public string VendorName { get; set; }

        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceValue { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? AWBNumber1 { get; set; }
        public DateTime? AWBDate1 { get; set; }
        public string? AWBNumber2 { get; set; }
        public DateTime? AWBDate2 { get; set; }
        public string? BENumber { get; set; }
        public DateTime? BEDate { get; set; }
        public decimal? TotalInvoiceValue { get; set; }
        public decimal? Freight { get; set; }
        public decimal? Insurance { get; set; }
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public string? GateEntryNo { get; set; }
        public decimal? CurrencyConversion { get; set; }
        public decimal? Transport { get; set; }
        public decimal? BECurrencyValue { get; set; }
        public Status Status { get; set; }
        public bool IsGrinCompleted { get; set; }
        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }
        public string? GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinPart> GrinParts { get; set; }
        public List<OtherCharge> OtherCharges { get; set; }
    }

    public class GrinPart
    {
        public int Id { get; set; }

        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }

        public PartType ItemType { get; set; }


        public decimal? Qty { get; set; }

        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }

        public string MftrItemNumber { get; set; }

        public string ManufactureBatchNumber { get; set; }
        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal POOrderQty { get; set; }

        public decimal POBalancedQty { get; set; }


        public decimal POUnitPrice { get; set; }

        public decimal AcceptedQty { get; set; }

        public decimal RejectedQty { get; set; }

        public decimal? AverageCost { get; set; }

        public string UOM { get; set; }
        public string? UOC { get; set; }
        public string? Remarks { get; set; }

        public GrinStatus Status { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        public bool IsCOCUploaded { get; set; } = false;

        public string? CoCUpload { get; set; }


        public decimal? SGST { get; set; }


        public decimal? IGST { get; set; }

       
        public decimal? CGST { get; set; }


        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? SerialNo { get; set; }
        public int GrinsId { get; set; }

        public List<ProjectNumber> ProjectNumbers { get; set; }
    }

    public class ProjectNumber
    {
        public int Id { get; set; }
        public string? projectNumber { get; set; }
        public decimal? ProjectQty { get; set; }

        public int GrinPartsId { get; set; }
    }

    public class OtherCharge
    {
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
        public int GrinsId { get; set; }
    }

    public class Iqc
    {
        public int Id { get; set; }
        public string? IQCNumber { get; set; }
        public string? GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<IqcConfirmationItem> IqcConfirmationItems { get; set; }
    }

    public class IqcConfirmationItem
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        public decimal ReceivedQty { get; set; }

        public decimal AcceptedQty { get; set; }


        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }
        public int IQCConfirmationId { get; set; }
    }

    public class Data1
    {
        public List<GrinsForServiceItems>? grinsForServiceItems { get; set; }
        public List<IQCForServiceItems>? iqcsForServiceItems { get; set; }
    }

    public class GrinForServiceItemsandIqcForServiceItemsDetail
    {
        public Data1 Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
    }
    public class GrinsForServiceItems
    {
        public int Id { get; set; }
        public string? GrinsForServiceItemsNumber { get; set; }
        public string VendorName { get; set; }
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceValue { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? AWBNumber1 { get; set; }
        public DateTime? AWBDate1 { get; set; }
        public string? AWBNumber2 { get; set; }
        public DateTime? AWBDate2 { get; set; }
        public string? BENumber { get; set; }
        public DateTime? BEDate { get; set; }
        public decimal? TotalInvoiceValue { get; set; }
        public decimal? Freight { get; set; }
        public decimal? Insurance { get; set; }
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public string? GateEntryNo { get; set; }
        public decimal? CurrencyConversion { get; set; }
        public decimal? Transport { get; set; }
        public decimal? BECurrencyValue { get; set; }
        public Status Status { get; set; }
        public bool IsGrinsForServiceItemsCompleted { get; set; }
        public bool IsIqcForServiceItemsCompleted { get; set; }
        public string? GrinsForServiceItemsDocuments { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinsForServiceItemsParts>? GrinsForServiceItemsParts { get; set; }
        public List<GrinsForServiceItemsOtherCharges>? GrinsForServiceItemsOtherCharges { get; set; }
    }
    public class GrinsForServiceItemsParts
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? LotNumber { get; set; }
        public PartType ItemType { get; set; }
        public decimal? Qty { get; set; }
        public string ItemDescription { get; set; }
        public string? PONumber { get; set; }
        public string MftrItemNumber { get; set; }
        public string ManufactureBatchNumber { get; set; }
        public bool IsIqcForServiceItemsCompleted { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal POOrderQty { get; set; }
        public decimal POBalancedQty { get; set; }
        public decimal POUnitPrice { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public decimal? AverageCost { get; set; }
        public string UOM { get; set; }
        public string? UOC { get; set; }
        public string? Remarks { get; set; }
        public GrinStatus Status { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public bool IsCOCUploaded { get; set; } = false;
        public string? CoCUpload { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? Duties { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? SerialNo { get; set; }
        public int GrinsForServiceItemsId { get; set; }        
        public List<GrinsForServiceItemsProjectNumbers>? GrinsForServiceItemsProjectNumbers { get; set; }
    }
    public class GrinsForServiceItemsProjectNumbers
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
        public int GrinsForServiceItemsPartsId { get; set; }
    }
    public class GrinsForServiceItemsOtherCharges
    {
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
        public int GrinsForServiceItemsId { get; set; }
    }
    public class IQCForServiceItems
    {
        public int Id { get; set; }
        public string? IQCForServiceItemsNumber { get; set; }
        public string? GrinsForServiceItemsNumber { get; set; }
        public int GrinsForServiceItemsId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public bool IsIqcForServiceItemsCompleted { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<IQCForServiceItems_Items>? IQCForServiceItems_Items { get; set; }
    }
    public class IQCForServiceItems_Items
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int GrinsForServiceItemsPartsId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
        public bool IsIqcForServiceItemsCompleted { get; set; }
        public int IQCForServiceItemsId { get; set; }
    }
}
