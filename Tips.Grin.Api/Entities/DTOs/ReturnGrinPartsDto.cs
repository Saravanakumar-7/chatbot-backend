using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class ReturnGrinPartsDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "PartNumber is required")]
        public string PartNumber { get; set; }

        [Required(ErrorMessage = "MftrNumber is required")]

        public string MftrNumber { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]

        public int UnitPrice { get; set; }

        [Required(ErrorMessage = "Qty is required")]
        [Precision(13, 2)]

        public decimal Qty { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ReturnGrinPartsDtoPost
    {
        [Required(ErrorMessage = "PartNumber is required")]
        public string PartNumber { get; set; }

        [Required(ErrorMessage = "MftrNumber is required")]

        public string MftrNumber { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage ="UnitPrice is required")]

        public int UnitPrice { get; set; }

        [Required(ErrorMessage = "Qty is required")]
        [Precision(13, 2)]

        public decimal Qty { get; set; }
        public string? Remarks { get; set; }
    }

    public class ReturnGrinPartsListDto
    {

        [Required(ErrorMessage = "PartNumber is required")]
        public string PartNumber { get; set; }

        [Required(ErrorMessage = "MftrNumber is required")]
        public string MftrNumber { get; set; }

        public string? Description { get; set; }

    }
}
