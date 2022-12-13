using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class BinningItemsDto
    {

        public int Id { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? MftrItemNo { get; set; }
        public string? ProjectNo { get; set; }
        public string? ManufactureBatchNo { get; set; }
        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        [Precision(18, 3)]
        public decimal? POOrderedQty { get; set; }
        [Precision(18, 3)]
        public decimal? POBalancedQty { get; set; }
        [Precision(18, 3)]
        public decimal? POUnitPrice { get; set; }
        
        public string? Uom { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        [Precision(18, 3)]
        public decimal? ReceivedQty { get; set; }
        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        //public List<BinningItemsDto>? binningItems { get; set; }
        public List<BinningLocationDto>? binningLocations { get; set; }
    }


    public class BinningItemsPostDto
    {

        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? MftrItemNo { get; set; }
        public string? ProjectNo { get; set; }
        public string? ManufactureBatchNo { get; set; }
        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        [Precision(18, 3)]
        public decimal? POOrderedQty { get; set; }
        [Precision(18, 3)]
        public decimal? POBalancedQty { get; set; }
        [Precision(18, 3)]
        public decimal? POUnitPrice { get; set; }
        
        public string? Uom { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        [Precision(18, 3)]
        public decimal? ReceivedQty { get; set; }
        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BinningLocationPostDto>? binningLocations { get; set; }

    }
    public class BinningItemsUpdateDto
    {

        public int Id { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? MftrItemNo { get; set; }
        public string? ProjectNo { get; set; }
        public string? ManufactureBatchNo { get; set; }
        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        [Precision(18, 3)]
        public decimal? POOrderedQty { get; set; }
        [Precision(18, 3)]
        public decimal? POBalancedQty { get; set; }
        [Precision(18, 3)]
        public decimal? POUnitPrice { get; set; }
        public string? Uom { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        [Precision(18, 3)]
        public decimal? ReceivedQty { get; set; }
        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BinningLocationUpdateDto>? binningLocations { get; set; }
    }
}