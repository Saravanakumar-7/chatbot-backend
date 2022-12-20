using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class BinningLocationDto
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public int? Quantity { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class BinningLocationPostDto
    {

        [StringLength(500, ErrorMessage = "BinningWarehouse can't be longer than 100 characters")]
        public string? Warehouse { get; set; }
        [StringLength(500, ErrorMessage = "BinningLocation can't be longer than 100 characters")]

        public string? Location { get; set; }

        public int? Quantity { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class BinningLocationUpdateDto
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessage = "BinningWarehouse can't be longer than 100 characters")]
        public string? Warehouse { get; set; }
        [StringLength(500, ErrorMessage = "BinningLocation can't be longer than 100 characters")]

        public string? Location { get; set; }

        public int? Quantity { get; set; }
        public string unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}