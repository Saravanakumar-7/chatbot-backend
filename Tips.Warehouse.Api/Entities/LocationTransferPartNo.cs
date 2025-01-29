using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class LocationTransferPartNo
    {
        [Key]
        public int Id { get; set; }
        public string? FromPartNumber { get; set; }
        public string? ToPartNumber { get; set; }
        public string? FromLotNumber { get; set; }
        public string? FromDescription { get; set; }
        public string? ToDescription { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToProjectNumber { get; set; }
        public string? FromLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromUOM { get; set; }
        public string? ToUOM { get; set; }
        public PartType FromPartType { get; set; }

        [Precision(13, 3)]
        public decimal? AvailableStockInLocation { get; set; }

        [Precision(13, 3)]
        public decimal? TransferQty { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
