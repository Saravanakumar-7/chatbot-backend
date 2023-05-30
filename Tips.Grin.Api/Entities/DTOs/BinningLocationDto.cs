using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class BinningLocationDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }

        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal Qty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class BinningLocationPostDto
    {
        public string? ProjectNumber { get; set; }

        public string? Warehouse { get; set; }
 
        public string? Location { get; set; }

        public decimal Qty { get; set; }
        
    }

    public class BinningLocationUpdateDto
    {
        public string? ProjectNumber { get; set; }

        public string? Warehouse { get; set; }
 
        public string? Location { get; set; }

        public decimal Qty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}