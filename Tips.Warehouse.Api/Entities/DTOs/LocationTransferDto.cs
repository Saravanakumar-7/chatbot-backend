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
        public string? FromPartType { get; set; }
        public string? ToPartType { get; set; }
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
        public string? FromPartType { get; set; }
        public string? ToPartType { get; set; }
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
        public string? FromPartType { get; set; }
        public string? ToPartType { get; set; }
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
}