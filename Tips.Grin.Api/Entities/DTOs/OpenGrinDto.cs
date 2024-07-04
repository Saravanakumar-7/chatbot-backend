using Entities;
using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinDto
    {
        public int Id { get; set; }
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinPartsDto> OpenGrinParts { get; set; }
    }
    public class OpenGrinPostDto
    {
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinPartsPostDto> OpenGrinParts { get; set; }
    }
    public class OpenGrinUpdateDto
    {
        //public int Id { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinPartsUpdateDto> OpenGrinParts { get; set; }
    }
    public class OGInventoryDtoPost
    {
        [Required]
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

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
        public bool IsStockAvailable { get; set; }
        public string Unit { get; set; }

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

    }
    public class OGInventoryTranctionDto
    {
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        public string MftrPartNumber { get; set; }


        public string Description { get; set; }

        public bool IsStockAvailable { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Issued_Quantity { get; set; }
        public string? UOM { get; set; }

        public string? Warehouse { get; set; }
        public string? From_Location { get; set; }
        public string? TO_Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

    }

    public class OpenGrinSearchDto
    {
        public List<string>? OpenGrinNumber { get; set; }
        public List<string>? SenderName { get; set; }
        //public List<string>? ReturnedBy { get; set; }
        public List<string>? ReceiptRefNo { get; set; }

    }
    public class OpenGrinDataListDto
    {
        public int Id { get; set; }
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
    }
    public class OpenGrinReportWithParamDto
    {
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiptRefNo { get; set; }
    }
    public class OpenGrinReportWithParamForTransDto
    {
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiptRefNo { get; set; }
        public string? ProjectNumber { get; set; }
        
    }
    public class OpenGrinInvDetails
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