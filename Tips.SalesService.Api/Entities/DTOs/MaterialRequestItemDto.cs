using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class MaterialRequestItemDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public string? Stock { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public bool IssueStatus { get; set; }

        [Precision(18, 3)]
        public decimal? RequiredQty { get; set; }

        
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }


    }
    public class MaterialRequestItemDtoPost
    {
        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public string? Stock { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public bool IssueStatus { get; set; }

        [Precision(18, 3)]
        public decimal? RequiredQty { get; set; }

    }
    public class MaterialRequestItemDtoUpdate
    {

      

        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public string? Stock { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public bool IssueStatus { get; set; }

        [Precision(18, 3)]
        public decimal? RequiredQty { get; set; }

       
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}
