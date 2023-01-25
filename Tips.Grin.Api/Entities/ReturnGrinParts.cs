using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class ReturnGrinParts
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "PartNumber is required")]
        public string PartNumber { get; set; }
        
        [Required(ErrorMessage = "MftrNumber is required")]

        public string MftrNumber { get; set; }

        public string? Description { get; set; }

        [Required]

        public int UnitPrice { get; set; }

        [Required(ErrorMessage = "Qty is required")]
        [Precision(13, 2)]

        public decimal Qty { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int ReturnGrinId { get; set; }
        public ReturnGrin? ReturnGrin { get; set; }

    }
}
