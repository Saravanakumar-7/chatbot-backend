using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities.DTOs
{
    public class OQCDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCPostDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCUpdateDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ShopOrderConfirmationItemNoListDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
    }
    public class ShopOrderConfirmationDetailsDto
    {
        public string? ShopOrderNumber { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public decimal? PendingQty { get; set; }

    }

    public class OQCIdNameList
    {
        public int Id { get; set; }

        public string? ShopOrderNumber { get; set; }
    }
    public class OQCSearchDto
    {
        //public List<string>? FGItemNumber { get; set; }
        //public List<string>? SAItemNumber { get; set; }
        public List<string>? ShopOrderNumber { get; set; }
        public List<decimal> ShopOrderQty { get; set; }
        public List<decimal> PendingQty { get; set; }
    }
    public class InventoryPostDto
    {
        [Required]
        public string PartNumber { get; set; }

        [Required]
        public string MftrPartNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProjectNumber { get; set; }
        [Required]
        public decimal Balance_Quantity { get; set; }
        [Required]
        public string? UOM { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }

        [Required]
        public string? Warehouse { get; set; }
        [Required]
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        [Required]
        public string? ReferenceID { get; set; }
        [Required]
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? LotNumber { get; set; }

    }
    public class InventoryTranctionDto
    {
        public string? PartNumber { get; set; }
        public string? LotNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public PartType? PartType { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        public string? UOM { get; set; }

        public DateTime? Issued_DateTime { get; set; }

        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

        public decimal? BOM_Version_No { get; set; }

        public string? From_Location { get; set; }
        public string? TO_Location { get; set; }

        public bool? ModifiedStatus { get; set; } = false;

        public string? Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public string? Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
    }
    public class OQCStock
    {
        public string ItemNumber { get; set; }
        public string ShopOrderNumber { get; set; }
        public decimal TotalAcceptedQty { get; set; }
    }
    public class OQCSPReportDto
    {
        public string? ItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
    }
    public class OQCSPReportForTransDto
    {
        public string? ItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? ProjectNumber { get; set; }
    }

    public class OqcItemMasterDetails
    {
        public Datas data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datas
    {
        public int id { get; set; }
        public string itemNumber { get; set; }
        public string? description { get; set; }
        public bool isActive { get; set; }
        //public bool isObsolete { get; set; }
        public int itemType { get; set; }
        public string? uom { get; set; }
        //public string commodity { get; set; }
        //public string hsn { get; set; }
        //public string materialGroup { get; set; }
        //public DateTime? validFrom { get; set; }
        //public DateTime? validTo { get; set; }
        //public string purchaseGroup { get; set; }
        //public string department { get; set; }
        //public string customerPartReference { get; set; }
        //public bool isPRRequired { get; set; }
        //public string poMaterialType { get; set; }
        //public bool openGrin { get; set; }
        //public bool isCustomerSuppliedItem { get; set; }
        //public string drawingNo { get; set; }
        //public string docRet { get; set; }
        //public string revNo { get; set; }
        //public bool isCocRequired { get; set; }
        //public bool isRohsItem { get; set; }
        //public bool isShelfLife { get; set; }
        //public bool isReachItem { get; set; }
        //public int imageUpload { get; set; }
        //public string fileUpload { get; set; }
        //public decimal netWeight { get; set; }
        //public string netUom { get; set; }
        //public decimal grossWeight { get; set; }
        //public string grossUom { get; set; }
        //public decimal volume { get; set; }
        //public string volumeUom { get; set; }
        //public decimal size { get; set; }
        //public string footPrint { get; set; }
        public decimal? min { get; set; }
        public decimal? max { get; set; }
        //public string leadTime { get; set; }
        //public string reorder { get; set; }
        //public string twoBin { get; set; }
        //public bool kanban { get; set; }
        //public bool isEsd { get; set; }
        //public bool isFifo { get; set; }
        //public bool isLifo { get; set; }
        //public bool isCycleCount { get; set; }
        //public bool isHazardousMaterial { get; set; }
        //public string expiry { get; set; }
        //public string inspectionInterval { get; set; }
        //public string specialInstructions { get; set; }
        //public string shippingInstruction { get; set; }
        //public bool isIQCRequired { get; set; }
        //public int grProcessing { get; set; }
        //public string batchSize { get; set; }
        //public string costCenter { get; set; }
        //public decimal stdCost { get; set; }
        //public string costingMethod { get; set; }
        //public bool valuation { get; set; }
        //public bool depreciation { get; set; }
        //public bool pfo { get; set; }
        //public string unit { get; set; }
        //public string remarksToVendor { get; set; }
        //public string createdBy { get; set; }
        //public DateTime? createdOn { get; set; }
        //public string lastModifiedBy { get; set; }
        //public DateTime? lastModifiedOn { get; set; }
        public Itemmasteralternate[] itemmasterAlternate { get; set; }
        //public Itemmasterwarehouse[] itemMasterWarehouse { get; set; }
        //public Itemmasterapprovedvendor[] itemMasterApprovedVendor { get; set; }
        //public Itemmasterrouting[] itemMasterRouting { get; set; }
    }

    public class Itemmasteralternate
    {
        public int id { get; set; }
        public string manufacturerPartNo { get; set; }
        public string? manufacturer { get; set; }
        public bool? alternateSource { get; set; }
        public bool? isDefault { get; set; }
        //public string createdBy { get; set; }
        //public DateTime? createdOn { get; set; }
        //public string lastModifiedBy { get; set; }
        //public DateTime? lastModifiedOn { get; set; }
    }

    //public class Itemmasterwarehouse
    //{
    //    public int id { get; set; }
    //    public string wareHouse { get; set; }
    //    public bool isActive { get; set; }
    //    //public string createdBy { get; set; }
    //    //public DateTime? createdOn { get; set; }
    //    //public string lastModifiedBy { get; set; }
    //    //public DateTime? lastModifiedOn { get; set; }
    //}

    //public class Itemmasterapprovedvendor
    //{
    //    public int id { get; set; }
    //    public string vendorCode { get; set; }
    //    public string vendorName { get; set; }
    //    public string shareOfBusiness { get; set; }
    //    //public string createdBy { get; set; }
    //    //public DateTime? createdOn { get; set; }
    //    //public string lastModifiedBy { get; set; }
    //    //public DateTime? lastModifiedOn { get; set; }
    //}

    //public class Itemmasterrouting
    //{
    //    public int id { get; set; }
    //    public string processStep { get; set; }
    //    public string process { get; set; }
    //    public string routingDescription { get; set; }
    //    public string machineHours { get; set; }
    //    public string laborHours { get; set; }
    //    public bool isRoutingActive { get; set; }
    //    //public string createdBy { get; set; }
    //    //public DateTime? createdOn { get; set; }
    //    //public string lastModifiedBy { get; set; }
    //    //public DateTime? lastModifiedOn { get; set; }
    //}

}
