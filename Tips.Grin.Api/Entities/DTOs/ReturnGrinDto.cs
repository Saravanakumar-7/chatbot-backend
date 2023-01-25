using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class ReturnGrinDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "CustomerName is required")]
        public string Customername { get; set; }

        [Required(ErrorMessage = "CustomerId is required")]

        public string CustomerId { get; set; }

        [Required(ErrorMessage = "GrinNumber is required")]

        public string GrinNumber { get; set; }


        public string? DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string? ReturnGrinDocuments { get; set; }
        public string? Remarks { get; set; }

        public string Unit { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ReturnGrinPartsDto>? ReturnGrinParts { get; set; }

    }

    public class ReturnGrinDtoPost
    {
        [Required(ErrorMessage = "CustomerName is required")]
        public string Customername { get; set; }

        [Required(ErrorMessage = "CustomerId is required")]

        public string CustomerId { get; set; }

        [Required(ErrorMessage = "GrinNumber is required")]

        public string GrinNumber { get; set; }


        public string? DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string? ReturnGrinDocuments { get; set; }
        public string? Remarks { get; set; }

        public List<ReturnGrinPartsDtoPost>? ReturnGrinParts { get; set; }
    }
}
