using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tips.Grin.Api.Entities.Enums;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCReturnToVendorPostDto
    {
        public string? IQCNumber { get; set; }
        public string? GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public List<IQCReturnToVendorItemsPostDto> iQCReturnToVendorItemsPostDtos { get; set; }
    }

    public class InventoryDtoDetails
    {
        public List<Datum> data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datum
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? UOM { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string Unit { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class InventoryUpdateDto
    {
        public string PartNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? UOM { get; set; }
        public string Warehouse { get; set; }
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; } = "Bought Out";
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? SerialNo { get; set; }
        public string Unit { get; set; }
    }

    public class PurchaseOrderReturns
    {
        public string PurchaseOrderNo { get; set; }
        public List<PurchaseOrderItems> purchaseOrderItems { get; set; }    
    }
    public class PurchaseOrderItems
    {
        public string ItemNumber { get; set; }
        public decimal ReturnQty { get; set; }
        public List<PurchaseOrderProject> purchaseOrderProjects { get; set; }
    }
    public class PurchaseOrderProject {
        public string ProjectNumber { get; set; }
        public decimal? ReturnQty { get; set; }
    }

    //public class PurchaseOrderDtoDetails
    //{
    //    public List<PurchaseOrderDetails> data { get; set; }
    //    public string message { get; set; }
    //    public bool success { get; set; }
    //    public int statusCode { get; set; }
    //}
    //public class PurchaseOrderDetails
    //{
    //    public int Id { get; set; }
    //    public string? PONumber { get; set; }
    //    public string? AmountInWords { get; set; }
    //    public DateTime PODate { get; set; }
    //    public int? RevisionNumber { get; set; }
    //    public string? ProcurementType { get; set; }
    //    public string? Currency { get; set; }
    //    public string? CompanyAliasName { get; set; }
    //    public string? Transports { get; set; }
    //    public string? Other { get; set; }
    //    public string? VendorName { get; set; }
    //    public string? VendorId { get; set; }
    //    public string? VendorNumber { get; set; }
    //    public string? QuotationReferenceNumber { get; set; }
    //    public DateTime? QuotationDate { get; set; }
    //    public string? VendorAddress { get; set; }
    //    public string? DeliveryTerms { get; set; }
    //    public string? PaymentTerms { get; set; }
    //    public string? ShippingMode { get; set; }
    //    public string? ShipTo { get; set; }
    //    public string? BillTo { get; set; }
    //    public int? BillToId { get; set; }
    //    public int? ShipToId { get; set; }
    //    public string? RetentionPeriod { get; set; }
    //    public string? SpecialTermsAndConditions { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public bool POApprovalI { get; set; } = false;
    //    public string? POApprovedIBy { get; set; }
    //    public DateTime POApprovedIDate { get; set; }
    //    public bool POApprovalII { get; set; } = false;
    //    public string? POApprovedIIBy { get; set; }
    //    public DateTime POApprovedIIDate { get; set; }
    //    [DefaultValue(false)]
    //    public bool POApprovalIII { get; set; }
    //    public string? POApprovedIIIBy { get; set; }
    //    public DateTime? POApprovedIIIDate { get; set; }
    //    [DefaultValue(false)]
    //    public bool POApprovalIV { get; set; }
    //    public string? POApprovedIVBy { get; set; }
    //    public DateTime? POApprovedIVDate { get; set; }
    //    public bool IsDeleted { get; set; } = false;
    //    public bool TallyStatus { get; set; } = false;

    //    [DefaultValue(0)]
    //    public Status Status { get; set; }

    //    [DefaultValue(false)]
    //    public bool IsShortClosed { get; set; }
    //    public string? ShortClosedBy { get; set; }
    //    public DateTime? ShortClosedOn { get; set; }
    //    public PoStatus PoStatus { get; set; }
    //    [DefaultValue(false)]
    //    public bool PoConfirmationStatus { get; set; }
    //    public int? ApprovalCount { get; set; }
    //    public string? Unit { get; set; }
    //    public string? CreatedBy { get; set; }
    //    public DateTime? CreatedOn { get; set; }
    //    public bool IsModified { get; set; } = false;
    //    public string? LastModifiedBy { get; set; }
    //    public DateTime? LastModifiedOn { get; set; }
    //    public string? POFiles { get; set; }
    //    public List<PoItemDetails>? POItems { get; set; }

    //}
    //public class PoItemDetails
    //{
    //    public int Id { get; set; }
    //    public string? ItemNumber { get; set; }
    //    public string? MftrItemNumber { get; set; }
    //    public string? Description { get; set; }
    //    public string? UOM { get; set; }
    //    [Precision(18, 3)]
    //    public decimal UnitPrice { get; set; }
    //    [Precision(13, 3)]
    //    public decimal Qty { get; set; }
    //    public string? PONumber { get; set; }
    //    public decimal BalanceQty { get; set; }
    //    [Precision(13, 3)]
    //    public decimal ReceivedQty { get; set; }
    //    public PartType? PartType { get; set; }
    //    public string? SpecialInstruction { get; set; }
    //    public decimal? Allowance { get; set; }
    //    public bool IsTechnicalDocsRequired { get; set; }
    //    public bool PoPartsStatus { get; set; }

    //    [Precision(13, 3)]
    //    public decimal SGST { get; set; }
    //    [Precision(13, 3)]
    //    public decimal CGST { get; set; }
    //    [Precision(13, 3)]
    //    public decimal IGST { get; set; }
    //    [Precision(13, 3)]
    //    public decimal UTGST { get; set; }


    //    [Precision(13, 3)]
    //    public decimal? SubTotal { get; set; }

    //    [Precision(13, 3)]
    //    public decimal TotalWithTax { get; set; }
    //    public string? ShortClosedBy { get; set; }
    //    public DateTime? ShortClosedOn { get; set; }
    //    public string? CreatedBy { get; set; }
    //    public DateTime? CreatedOn { get; set; }
    //    public string? LastModifiedBy { get; set; }
    //    public DateTime? LastModifiedOn { get; set; }
    //    public PoStatus PoStatus { get; set; }
    //    public string? ReasonforShortClose { get; set; }
    //    public string? Remarks { get; set; }
    //    public string? drawingRevNo { get; set; }
    //    public int PurchaseOrderId { get; set; }

    //    public List<PoAddProjectDetails>? POAddprojects { get; set; }

    //}
    //public class PoAddProjectDetails
    //{
    //    public int Id { get; set; }
    //    public string? ProjectNumber { get; set; }

    //    [Precision(13, 3)]
    //    public decimal ProjectQty { get; set; }
    //    public decimal BalanceQty { get; set; }
    //    [Precision(13, 3)]
    //    public decimal ReceivedQty { get; set; }
    //    public bool PoAddProjectStatus { get; set; }
    //    public int POItemDetailId { get; set; }

    //}

}
