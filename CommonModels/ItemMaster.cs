using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities
{
    public class ItemMaster
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
        public bool Tolerance { get; set; } = false;
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
        public string? CCT { get; set; }
        public string? BeamAngle { get; set; }
        public string? TrimColor { get; set; }
        public string? Reflector { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? Manufacture_Year { get; set; }
        public string? SubModel { get; set; }
        public string? Serial_Number { get; set; }
        public string? Asset_Number { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
       
        public DateTime? LastModifiedOn { get; set; }


        public List<ItemmasterAlternate>? ItemmasterAlternate { get; set; }
        public List<ItemMasterWarehouse>? ItemMasterWarehouse { get; set; }
        public List<ItemMasterApprovedVendor>? ItemMasterApprovedVendor { get; set; }     
        
        //public List<ImageUpload>? ImageUploads { get; set; }

        //public List<ItemMasterFileUpload>? ItemMasterFileUpload { get; set; }
        public List<ItemMasterRouting>? ItemMasterRouting { get; set; }
        public List<ItemMasterSchedules>? ItemMasterSchedules { get; set; }
    }

}

