using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class LocationTransferPartNoDto
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

    public class LocationTransferPartNoPostDto
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
}
