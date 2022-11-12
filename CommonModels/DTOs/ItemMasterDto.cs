using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ItemMasterDto
    {
        public long Id { get; set; }
        [MaxLength(100)]
        public string? ItemNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsObsolete { get; set; }
        [MaxLength(50)]
        public string? ItemType { get; set; }
        [MaxLength(20)]
        public string? Uom { get; set; }
        [MaxLength(50)]
        public string? Commodity { get; set; }
        [MaxLength(50)]
        public string? Hsn { get; set; }
        [MaxLength(100)]
        public string? MaterialGroup { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ValidFrom { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ValidTo { get; set; }
        [MaxLength(50)]
        public string? PurchaseGroup { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(100)]
        public string? CustomerPartReference { get; set; }
        
        public bool IsPRRequired { get; set; }
        [MaxLength(50)]
        public string? PoMaterialType { get; set; }
        
        public bool OpenGrin { get; set; }
        public bool IsCustomerSuppliedItem { get; set; }
        public string? DrawingNo { get; set; }
        public string? DocRet { get; set; }
        public string? RevNo { get; set; }
        
        public bool IsCocRequired { get; set; }
        
        public bool IsRohsItem { get; set; }
        
        public bool IsShelfLife { get; set; }
        
        public bool IsReachItem { get; set; }
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
        public string? Leadtime { get; set; }
        public string? Reorder { get; set; }
        public string? TwoBin { get; set; }
        public string? Kanban { get; set; }
        
        public bool IsEsd { get; set; }
        
        public bool IsFifo { get; set; }
        
        public bool IsLifo { get; set; }
        
        public bool IsCycleCount { get; set; }
        
        public bool IsHazardousMaterial { get; set; }
        public string? Expiry { get; set; }
        public string? InspectionInterval { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ShippingInstruction { get; set; }
        
        public bool IsIQCRequired { get; set; }
        public int GrProcessing { get; set; }
        public string? BatchSize { get; set; }
        public string? CostCenter { get; set; }
        public decimal StdCost { get; set; }
        public string? CostingMethod { get; set; }
        public bool Valuation { get; set; }
        public bool Depreciation { get; set; }
        public bool Pfo { get; set; }
        public string? CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy HH:mm:ss}")]
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy HH:mm:ss}")]
        public DateTime LastModifiedOn { get; set; }
        public List<ItemmasterAlternateDto> ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouseDto> ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendorDto> ItemMasterApprovedVendor { get; set; }
        public List<ItemMasterFileUploadDto> ItemMasterFileUpload { get; set; }
        public List<ItemMasterRoutingDto> ItemMasterRouting { get; set; }
    }

    public class ItemMasterDtoPost
    {
        [MaxLength(100)]
        public string? ItemNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsObsolete { get; set; }
        [MaxLength(50)]
        public string? ItemType { get; set; }
        [MaxLength(20)]
        public string? Uom { get; set; }
        [MaxLength(50)]
        public string? Commodity { get; set; }
        [MaxLength(50)]
        public string? Hsn { get; set; }
        [MaxLength(100)]
        public string? MaterialGroup { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ValidFrom { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ValidTo { get; set; }
        [MaxLength(50)]
        public string? PurchaseGroup { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(100)]
        public string? CustomerPartReference { get; set; }
        
        public bool IsPRRequired { get; set; }
        [MaxLength(50)]
        public string? PoMaterialType { get; set; }
        
        public bool OpenGrin { get; set; }
        public bool IsCustomerSuppliedItem { get; set; }
        public string? DrawingNo { get; set; }
        public string? DocRet { get; set; }
        public string? RevNo { get; set; }
        
        public bool IsCocRequired { get; set; }
        
        public bool IsRohsItem { get; set; }
        
        public bool IsShelfLife { get; set; }
        
        public bool IsReachItem { get; set; }
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
        public string? Leadtime { get; set; }
        public string? Reorder { get; set; }
        public string? TwoBin { get; set; }
        public string? Kanban { get; set; }
        
        public bool IsEsd { get; set; }
        
        public bool IsFifo { get; set; }
        
        public bool IsLifo { get; set; }
        
        public bool IsCycleCount { get; set; }
        
        public bool IsHazardousMaterial { get; set; }
        public string? Expiry { get; set; }
        public string? InspectionInterval { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ShippingInstruction { get; set; }
        
        public bool IsIQCRequired { get; set; }
        public int GrProcessing { get; set; }
        public string? BatchSize { get; set; }
        public string? CostCenter { get; set; }
        public decimal StdCost { get; set; }
        public string? CostingMethod { get; set; }
        public bool Valuation { get; set; }
        public bool Depreciation { get; set; }
        public bool Pfo { get; set; }
        public List<ItemmasterAlternateDtoPost> ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouseDtoPost> ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendorDtoPost> ItemMasterApprovedVendor { get; set; }
        public List<ItemMasterFileUploadDtoPost> ItemMasterFileUpload { get; set; }
        public List<ItemMasterRoutingDtoPost> ItemMasterRouting { get; set; }
    }

    public class ItemMasterDtoUpdate
    {
      
         public long Id { get; set; }
        [MaxLength(100)]
        public string? ItemNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsObsolete { get; set; }
        [MaxLength(50)]
        public string? ItemType { get; set; }
        [MaxLength(20)]
        public string? Uom { get; set; }
        [MaxLength(50)]
        public string? Commodity { get; set; }
        [MaxLength(50)]
        public string? Hsn { get; set; }
        [MaxLength(100)]
        public string? MaterialGroup { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ValidFrom { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ValidTo { get; set; }
        [MaxLength(50)]
        public string? PurchaseGroup { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(100)]
        public string? CustomerPartReference { get; set; }
        
        public bool IsPRRequired { get; set; }
        [MaxLength(50)]
        public string? PoMaterialType { get; set; }
        
        public bool OpenGrin { get; set; }
        public bool IsCustomerSuppliedItem { get; set; }
        public string? DrawingNo { get; set; }
        public string? DocRet { get; set; }
        public string? RevNo { get; set; }
        
        public bool IsCocRequired { get; set; }
        
        public bool IsRohsItem { get; set; }
        
        public bool IsShelfLife { get; set; }
        
        public bool IsReachItem { get; set; }
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
        public string? Leadtime { get; set; }
        public string? Reorder { get; set; }
        public string? TwoBin { get; set; }
        public string? Kanban { get; set; }
        
        public bool IsEsd { get; set; }
        
        public bool IsFifo { get; set; }
        
        public bool IsLifo { get; set; }
        
        public bool IsCycleCount { get; set; }
        
        public bool IsHazardousMaterial { get; set; }
        public string? Expiry { get; set; }
        public string? InspectionInterval { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ShippingInstruction { get; set; }
        
        public bool IsIQCRequired { get; set; }
        public int GrProcessing { get; set; }
        public string? BatchSize { get; set; }
        public string? CostCenter { get; set; }
        public decimal StdCost { get; set; }
        public string? CostingMethod { get; set; }
        public bool Valuation { get; set; }
        public bool Depreciation { get; set; }
        public bool Pfo { get; set; }
        public List<ItemmasterAlternateDtoUpdate> ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouseDtoUpdate> ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendorDtoUpdate> ItemMasterApprovedVendor { get; set; }
        public List<ItemMasterFileUploadDtoUpdate> ItemMasterFileUpload { get; set; }
        public List<ItemMasterRoutingDtoUpdate> ItemMasterRouting { get; set; }

    }

}
