using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class LocationTransferDto
    {
        public int Id { get; set; }
        public string? FromPartNumber { get; set; }
        public string? ToPartNumber { get; set; }
        public string? FromLocation { get; set; }
        public string? FromDescription { get; set; }
        public string? ToDescription { get; set; }
        [Precision(13, 3)]
        public decimal? AvailableStockInLocation { get; set; }
        public string? FromUOM { get; set; }
        public string? ToUOM { get; set; }
        public string ToLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? ToWarehouse { get; set; }
        public PartType FromPartType { get; set; }
        public PartType ToPartType { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal? TransferQty { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class LocationTransferPostDto
    {
        [Required(ErrorMessage = "FromPartNo is required")]
        [StringLength(500, ErrorMessage = "FromPartNo can't be longer than 500 characters")]
        public string FromPartNumber { get; set; }

        [Required(ErrorMessage = "ToPartNo is required")]
        [StringLength(500, ErrorMessage = "ToPartNo can't be longer than 500 characters")]
        public string ToPartNumber { get; set; }

        [Required(ErrorMessage = "FromLocation is required")]
        [StringLength(500, ErrorMessage = "FromLocation can't be longer than 500 characters")]
        public string FromLocation { get; set; }
        public string? FromDescription { get; set; }
        public string? ToDescription { get; set; }      

        [Precision(13, 3)]
        public decimal AvailableStockInLocation { get; set; }
        public string? FromUOM { get; set; }
        public string? ToUOM { get; set; }
        public string ToLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? ToWarehouse { get; set; }
        public PartType FromPartType { get; set; }
        public PartType ToPartType { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToProjectNumber { get; set; }

        [Precision(13, 3)]       
        public decimal TransferQty { get; set; }
        public string? Remarks { get; set; }

       
    }

    public class LocationTransferUpdateDto
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "FromPartNo is required")]
        [StringLength(500, ErrorMessage = "FromPartNo can't be longer than 500 characters")]
        public string FromPartNumber { get; set; }

        [Required(ErrorMessage = "ToPartNo is required")]
        [StringLength(500, ErrorMessage = "ToPartNo can't be longer than 500 characters")]
        public string ToPartNumber { get; set; }

        [Required(ErrorMessage = "FromLocation is required")]
        [StringLength(500, ErrorMessage = "FromLocation can't be longer than 500 characters")]
        public string FromLocation { get; set; }
        public string? FromDescription { get; set; }
        public string? ToDescription { get; set; }

        [Precision(13, 3)]
        public decimal AvailableStockInLocation { get; set; }
        public string? FromUOM { get; set; }
        public string? ToUOM { get; set; }
        public string ToLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? ToWarehouse { get; set; }
        public PartType FromPartType { get; set; }
        public PartType ToPartType { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal TransferQty { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
    }

    public class LocationTransferIdNameList
    {
        public int Id { get; set; }
        public string? FromPartNumber { get; set; }
        public string? ToPartNumber { get; set; }
    }
    public class LocationTransferSearchDto
    {
        public List<string>? FromPartNumber { get; set; }
        public List<string>? ToPartNumber { get; set; }
        public List<string>? FromUOM { get; set; }
        public List<string>? ToUOM { get; set; }
        public List<string>? FromLocation { get; set; }
        public List<string> ToLocation { get; set; }
    }

    public class LocationTransferFromDto
    {
        public string? FromProject { get; set; }
        public string? FromLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromLotNumber {  get; set; }
        public decimal? AvailableStock { get; set; }

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

        [Required]
        public string? Warehouse { get; set; }
        public bool IsStockAvailable { get; set; }        
        public string? Unit { get; set; }
        [Required]
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        [Required]
        public string? ReferenceID { get; set; }
        [Required]
        public string? ReferenceIDFrom { get; set; }
        public string? shopOrderNo { get; set; }

    }
    public class LocationTransferSPReportDTO
    {
        public string? FromKpn { get; set; }
        public string? FromUOM { get; set; }
        public string? FromDescription { get; set; }
        public decimal? FromQty { get; set; }
        public string? FromLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToKPN { get; set; }
        public PartType ToPartType { get; set; }
        public string? ToUOM { get; set; }
        public string? ToDescription { get; set; }
        public decimal? ToQty { get; set; }
        public string? ToLocation { get; set; }
        public string? ToWarehouse { get; set; }
        public string? ToProjectNumber { get; set; }
        public string? Remarks { get; set; }
    }
    public class LocationTransferSPReportWithParamDTO
    {
        public string? FromPartNumber { get; set; }
        public string? FromPartType { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromLocation { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToPartNumber { get; set; }
        public string? ToPartType { get; set; }
        public string? ToWarehouse { get; set; }
        public string? ToLocation { get; set; }
        public string? ToProjectNumber { get; set; }

    }
    public class LocationTransItemMasterDetails
    {
        public Datass data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datass
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
        public Itemmasteralternates[] itemmasterAlternate { get; set; }
        //public Itemmasterwarehouse[] itemMasterWarehouse { get; set; }
        //public Itemmasterapprovedvendor[] itemMasterApprovedVendor { get; set; }
        //public Itemmasterrouting[] itemMasterRouting { get; set; }
    }

    public class Itemmasteralternates
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