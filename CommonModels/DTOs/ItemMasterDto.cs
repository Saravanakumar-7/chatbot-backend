using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities.DTOs
{
    public class ItemMasterDto
    {
        [Key]
        [Column("ItemMasterId")]
        public long Id { get; set; }
        [Required(ErrorMessage = "ItemNumber is required")]
        [MaxLength(100)]
        public string? ItemNumber { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500)]
        public string? Description { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [DefaultValue(false)]
        public bool IsObsolete { get; set; }
        public PartType ItemType { get; set; }
        [MaxLength(20)]
        public string? Uom { get; set; }
        [MaxLength(50)]
        public string? Commodity { get; set; }
        [MaxLength(50)]
        public string? Hsn { get; set; }
        [MaxLength(100)]
        public string? MaterialGroup { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? ValidFrom { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? ValidTo { get; set; }
        [MaxLength(50)]
        public string? PurchaseGroup { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(100)]
        public string? CustomerPartReference { get; set; }
        [DefaultValue(true)]
        public bool IsPRRequired { get; set; }
        [MaxLength(50)]
        public string? PoMaterialType { get; set; }
        [DefaultValue(false)]
        public bool OpenGrin { get; set; }
        public bool IsCustomerSuppliedItem { get; set; }
        public string? DrawingNo { get; set; }
        public string? DocRet { get; set; }
        public string? RevNo { get; set; }
        [DefaultValue(false)]
        public bool IsCocRequired { get; set; }
        [DefaultValue(false)]
        public bool IsRohsItem { get; set; }
        [DefaultValue(false)]
        public bool IsShelfLife { get; set; }
        [DefaultValue(false)]
        public bool IsReachItem { get; set; }

        public int? ImageUpload { get; set; }

        public string? FileUpload { get; set; }

        public decimal NetWeight { get; set; }
        public string? NetUom { get; set; }
        public decimal GrossWeight { get; set; }
        public string? GrossUom { get; set; }
        public decimal Volume { get; set; }
        public string? VolumeUom { get; set; }
        public decimal Size { get; set; }
        public string? FootPrint { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string? LeadTime { get; set; }
        public string? Reorder { get; set; }
        public string? TwoBin { get; set; }
        public bool Kanban { get; set; }
        [DefaultValue(false)]
        public bool IsEsd { get; set; }
        [DefaultValue(false)]
        public bool IsFifo { get; set; }
        [DefaultValue(false)]
        public bool IsLifo { get; set; }
        [DefaultValue(false)]
        public bool IsCycleCount { get; set; }
        [DefaultValue(false)]
        public bool IsHazardousMaterial { get; set; }
        public string? Expiry { get; set; }
        public string? InspectionInterval { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ShippingInstruction { get; set; }
        [DefaultValue(false)]
        public bool IsIQCRequired { get; set; }
        public int GrProcessing { get; set; }
        public string? BatchSize { get; set; }
        public string? CostCenter { get; set; }
        public decimal StdCost { get; set; }
        public string? CostingMethod { get; set; }
        public bool Valuation { get; set; }
        public bool Depreciation { get; set; }
        public bool Pfo { get; set; }
        public string Unit { get; set; }
        public string? RemarksToVendor { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ItemmasterAlternateDto>? ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouseDto>? ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendorDto>? ItemMasterApprovedVendor { get; set; }
        //public List<ItemMasterFileUploadDto>? ItemMasterFileUpload { get; set; }
        public List<ItemMasterRoutingDto>? ItemMasterRouting { get; set; }
    }

    public class ItemMasterDtoPost
    {
        [Required(ErrorMessage = "ItemNumber is required")]
        [MaxLength(100)]
        public string? ItemNumber { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500)]
        public string? Description { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [DefaultValue(false)]
        public bool IsObsolete { get; set; }
        public PartType ItemType { get; set; }
        [MaxLength(20)]
        public string? Uom { get; set; }
        [MaxLength(50)]
        public string? Commodity { get; set; }
        [MaxLength(50)]
        public string? Hsn { get; set; }
        [MaxLength(100)]
        public string? MaterialGroup { get; set; }
        
        public DateTime? ValidFrom { get; set; }
        
        public DateTime? ValidTo { get; set; }
        [MaxLength(50)]
        public string? PurchaseGroup { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(100)]
        public string? CustomerPartReference { get; set; }
        [DefaultValue(true)]
        public bool IsPRRequired { get; set; }
        [MaxLength(50)]
        public string? PoMaterialType { get; set; }
        [DefaultValue(false)]
        public bool OpenGrin { get; set; }
        public bool IsCustomerSuppliedItem { get; set; }
        public string? DrawingNo { get; set; }
        public string? DocRet { get; set; }
        public string? RevNo { get; set; }
        [DefaultValue(false)]
        public bool IsCocRequired { get; set; }
        [DefaultValue(false)]
        public bool IsRohsItem { get; set; }
        [DefaultValue(false)]
        public bool IsShelfLife { get; set; }
        [DefaultValue(false)]
        public bool IsReachItem { get; set; }

        public string? FileUpload { get; set; }

        public int? ImageUpload { get; set; }

        public decimal NetWeight { get; set; }
        public string? NetUom { get; set; }
        public decimal GrossWeight { get; set; }
        public string? GrossUom { get; set; }
        public decimal Volume { get; set; }
        public string? VolumeUom { get; set; }
        public decimal Size { get; set; }
        public string? FootPrint { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string? LeadTime { get; set; }
        public string? Reorder { get; set; }
        public string? TwoBin { get; set; }
        public bool Kanban { get; set; }
        [DefaultValue(false)]
        public bool IsEsd { get; set; }
        [DefaultValue(false)]
        public bool IsFifo { get; set; }
        [DefaultValue(false)]
        public bool IsLifo { get; set; }
        [DefaultValue(false)]
        public bool IsCycleCount { get; set; }
        [DefaultValue(false)]
        public bool IsHazardousMaterial { get; set; }
        public string? Expiry { get; set; }
        public string? InspectionInterval { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ShippingInstruction { get; set; }
        [DefaultValue(false)]
        public bool IsIQCRequired { get; set; }
        public int GrProcessing { get; set; }
        public string? BatchSize { get; set; }
        public string? CostCenter { get; set; }
        public decimal StdCost { get; set; }
        public string? CostingMethod { get; set; }
        public bool Valuation { get; set; }
        public bool Depreciation { get; set; }
        public bool Pfo { get; set; }
        public string? RemarksToVendor { get; set; }


        public List<ItemmasterAlternateDtoPost>? ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouseDtoPost>? ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendorDtoPost>? ItemMasterApprovedVendor { get; set; }
        //public List<ItemMasterFileUploadDtoPost>? ItemMasterFileUpload { get; set; }
        public List<ItemMasterRoutingDtoPost>? ItemMasterRouting { get; set; }
    }

    public class ItemMasterDtoUpdate
    {
        [Key]
        [Column("ItemMasterId")]
        public long Id { get; set; }
        [Required(ErrorMessage = "ItemNumber is required")]
        [MaxLength(100)]
        public string? ItemNumber { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500)]
        public string? Description { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [DefaultValue(false)]
        public bool IsObsolete { get; set; }
        public PartType ItemType { get; set; }
        [MaxLength(20)]
        public string? Uom { get; set; }
        [MaxLength(50)]
        public string? Commodity { get; set; }
        [MaxLength(50)]
        public string? Hsn { get; set; }
        [MaxLength(100)]
        public string? MaterialGroup { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? ValidFrom { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? ValidTo { get; set; }
        [MaxLength(50)]
        public string? PurchaseGroup { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(100)]
        public string? CustomerPartReference { get; set; }
        [DefaultValue(true)]
        public bool IsPRRequired { get; set; }
        [MaxLength(50)]
        public string? PoMaterialType { get; set; }
        [DefaultValue(false)]
        public bool OpenGrin { get; set; }
        public bool IsCustomerSuppliedItem { get; set; }
        public string? DrawingNo { get; set; }
        public string? DocRet { get; set; }
        public string? RevNo { get; set; }
        [DefaultValue(false)]
        public bool IsCocRequired { get; set; }
        [DefaultValue(false)]
        public bool IsRohsItem { get; set; }
        [DefaultValue(false)]
        public bool IsShelfLife { get; set; }
        [DefaultValue(false)]
        public bool IsReachItem { get; set; }

        public string? FileUpload { get; set; }

        public int? ImageUpload { get; set; }

        public decimal NetWeight { get; set; }
        public string? NetUom { get; set; }
        public decimal GrossWeight { get; set; }
        public string? GrossUom { get; set; }
        public decimal Volume { get; set; }
        public string? VolumeUom { get; set; }
        public decimal Size { get; set; }
        public string? FootPrint { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string? LeadTime { get; set; }
        public string? Reorder { get; set; }
        public string? TwoBin { get; set; }
        public bool Kanban { get; set; }
        [DefaultValue(false)]
        public bool IsEsd { get; set; }
        [DefaultValue(false)]
        public bool IsFifo { get; set; }
        [DefaultValue(false)]
        public bool IsLifo { get; set; }
        [DefaultValue(false)]
        public bool IsCycleCount { get; set; }
        [DefaultValue(false)]
        public bool IsHazardousMaterial { get; set; }
        public string? Expiry { get; set; }
        public string? InspectionInterval { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ShippingInstruction { get; set; }
        [DefaultValue(false)]
        public bool IsIQCRequired { get; set; }
        public int GrProcessing { get; set; }
        public string? BatchSize { get; set; }
        public string? CostCenter { get; set; }
        public decimal StdCost { get; set; }
        public string? CostingMethod { get; set; }
        public bool Valuation { get; set; }
        public bool Depreciation { get; set; }
        public bool Pfo { get; set; }
        public string? RemarksToVendor { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }            
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ItemmasterAlternateDtoUpdate>? ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouseDtoUpdate>? ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendorDtoUpdate>? ItemMasterApprovedVendor { get; set; }
        //public List<ItemMasterFileUploadDtoUpdate>? ItemMasterFileUpload { get; set; }
        public List<ItemMasterRoutingDtoUpdate>? ItemMasterRouting { get; set; }

    }
    public class GetDownloadUrlDtos
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string? FileByte { get; set; }
        public string DownloadUrl { get; set; }
    }
    public class ItemMasterIdNoListDto
    {
        public long id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
    }
    public class ItemMasterMtrPartNoDto
    {
        public string? ManufacturerPartNo { get; set; }
        public string? Description { get; set; }
        public string? Uom { get; set; }
    }
    public class ItemMasterSearchDto
    {
        public List<PartType> ItemType { get; set; }
        public List<string>? ItemNumber { get; set; }
        public List<string>? Commodity { get; set; }
        public List<string>? MaterialGroup { get; set; }
        public List<string>? PurchaseGroup { get; set; }
        public List<string>? Department { get; set; }
    }
    public class ItemNoListDtos
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
    }
    public class ItemNoListDto
    {
        public string? ItemNumber { get; set; }
    }
    public class ItemMasterReportDto
    {
        [Key]
        [Column("ItemMasterId")]
        public long Id { get; set; }
        [Required(ErrorMessage = "ItemNumber is required")]
        [MaxLength(100)]
        public string? ItemNumber { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500)]
        public string? Description { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [DefaultValue(false)]
        public bool IsObsolete { get; set; }
        public PartType ItemType { get; set; }
        [MaxLength(20)]
        public string? Uom { get; set; }
        [MaxLength(50)]
        public string? Commodity { get; set; }
        [MaxLength(50)]
        public string? Hsn { get; set; }
        [MaxLength(100)]
        public string? MaterialGroup { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? ValidFrom { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? ValidTo { get; set; }
        [MaxLength(50)]
        public string? PurchaseGroup { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(100)]
        public string? CustomerPartReference { get; set; }
        [DefaultValue(true)]
        public bool IsPRRequired { get; set; }
        [MaxLength(50)]
        public string? PoMaterialType { get; set; }
        [DefaultValue(false)]
        public bool OpenGrin { get; set; }
        public bool IsCustomerSuppliedItem { get; set; }
        public string? DrawingNo { get; set; }
        public string? DocRet { get; set; }
        public string? RevNo { get; set; }
        [DefaultValue(false)]
        public bool IsCocRequired { get; set; }
        [DefaultValue(false)]
        public bool IsRohsItem { get; set; }
        [DefaultValue(false)]
        public bool IsShelfLife { get; set; }
        [DefaultValue(false)]
        public bool IsReachItem { get; set; }

        public int? ImageUpload { get; set; }

        public string? FileUpload { get; set; }

        public decimal NetWeight { get; set; }
        public string? NetUom { get; set; }
        public decimal GrossWeight { get; set; }
        public string? GrossUom { get; set; }
        public decimal Volume { get; set; }
        public string? VolumeUom { get; set; }
        public decimal Size { get; set; }
        public string? FootPrint { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string? LeadTime { get; set; }
        public string? Reorder { get; set; }
        public string? TwoBin { get; set; }
        public bool Kanban { get; set; }
        [DefaultValue(false)]
        public bool IsEsd { get; set; }
        [DefaultValue(false)]
        public bool IsFifo { get; set; }
        [DefaultValue(false)]
        public bool IsLifo { get; set; }
        [DefaultValue(false)]
        public bool IsCycleCount { get; set; }
        [DefaultValue(false)]
        public bool IsHazardousMaterial { get; set; }
        public string? Expiry { get; set; }
        public string? InspectionInterval { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ShippingInstruction { get; set; }
        [DefaultValue(false)]
        public bool IsIQCRequired { get; set; }
        public int GrProcessing { get; set; }
        public string? BatchSize { get; set; }
        public string? CostCenter { get; set; }
        public decimal StdCost { get; set; }
        public string? CostingMethod { get; set; }
        public bool Valuation { get; set; }
        public bool Depreciation { get; set; }
        public bool Pfo { get; set; }
        public string Unit { get; set; }
        public string? RemarksToVendor { get; set; }

        public string? CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy HH:mm:ss}")]
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy HH:mm:ss}")]
        public DateTime LastModifiedOn { get; set; }
        public List<ItemmasterAlternateReportDto>? ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouseReportDto>? ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendorReportDto>? ItemMasterApprovedVendor { get; set; }
        //public List<ItemMasterFileUploadDto>? ItemMasterFileUpload { get; set; }
        public List<ItemMasterRoutingReportDto>? ItemMasterRouting { get; set; }
    }

    public class ItemWithPartTypeDto 
    {
        public string ItemNumber { get; set; }

        public PartType PartType { get; set; }
    }

}
