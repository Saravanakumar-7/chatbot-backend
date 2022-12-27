using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EnggAlternatesDto
    {
        public int Id { get; set; }

        public string ChildItemNumber { get; set; }
        public string ChildItemAlternateNumber { get; set; }

        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal QuantityPer { get; set; }

        public string? Description { get; set; }

        public string? Remark { get; set; }

        public string? Version { get; set; }

        public string? ProbabilityOfUsage { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class EnggAlternatesPostDto
    {
        [Required(ErrorMessage = "ChildItemNumber is required")]
        public string ChildItemNumber { get; set; }

        [Required(ErrorMessage = "ChildItemAlternateNumber is required")]
        public string ChildItemAlternateNumber { get; set; }

        public string? UOM { get; set; }

        [Required(ErrorMessage = "QuantityPer is required")]
        [Precision(13, 3)]
        public int QuantityPer { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Remark can't be longer than 500 characters")]
        public string? Remarks { get; set; }

        public string? Version { get; set; }

        public string? ProbabilityOfUsage { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class EnggAlternatesUpdateDto
    {
    public int Id { get; set; }

    [Required(ErrorMessage = "ChildItemNumber is required")]
    public string ChildItemNumber { get; set; }

    [Required(ErrorMessage = "ChildItemAlternateNumber is required")]
    public string ChildItemAlternateNumber { get; set; }

    public string? UOM { get; set; }

    [Required(ErrorMessage = "QuantityPer is required")]
    [Precision(13,3)]
    public decimal QuantityPer { get; set; }

    [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
    public string? Description { get; set; }

    [StringLength(500, ErrorMessage = "Remark can't be longer than 500 characters")]
    public string? Remarks { get; set; }

    public string? Version { get; set; }

    public string? ProbabilityOfUsage { get; set; }

    [DefaultValue(true)]
    public bool IsActive { get; set; }
    public string Unit { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

}
