using Entities.Enums;
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
    public class EnggChildItemDto
    {
        public int Id { get; set; }

        public string ItemNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal Quantity { get; set; }

        public string? Description { get; set; }
        public PartType PartType { get; set; }

        public string? Remarks { get; set; }

        public string? Version { get; set; }

        public string? ScrapAllowance { get; set; }

        public string? ScrapAllowanceType { get; set; }
        public string? CustomFields { get; set; }
        public string? Designator { get; set; }
        public string? FootPrint { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggAlternatesDto>? EnggAlternatesDtos { get; set; }



    }
    public class EnggChildItemPostDto
    {
        [Required(ErrorMessage = "ItemNumber is required")]
        public string ItemNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? UOM { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Precision(13, 3)]
        public decimal Quantity { get; set; }       
        public string? Description { get; set; }
        public PartType PartType { get; set; }
        public string? Remarks { get; set; }
        public string? Version { get; set; }
        public string? ScrapAllowance { get; set; }
        public string? ScrapAllowanceType { get; set; }
        public string? CustomFields { get; set; }
        public string? Designator { get; set; }
        public string? FootPrint { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public List<EnggAlternatesPostDto>? EnggAlternatesPostDtos { get; set; }

    }
    public class EnggChildItemPostDtoWithValidation
    {
        [Required(ErrorMessage = "ItemNumber is required")]
        public string ItemNumber { get; set; }
        [Required(ErrorMessage = "MftrItemNumbers is required")]
        public string MftrItemNumbers { get; set; }
        [Required(ErrorMessage = "UOM is required")]
        public string UOM { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Precision(13, 3)]
        public decimal Quantity { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "PartType is required")]
        public PartType PartType { get; set; }
        public string? Remarks { get; set; }
        public string? Version { get; set; }
        public string? ScrapAllowance { get; set; }
        public string? ScrapAllowanceType { get; set; }
        public string? CustomFields { get; set; }
        public string? Designator { get; set; }
        public string? FootPrint { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public List<EnggAlternatesPostDto>? EnggAlternatesPostDtos { get; set; }

    }
    public class EnggChildItemUpdateDto
    {
         

        [Required(ErrorMessage = "ItemNumber is required")]
        public string ItemNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? UOM { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Precision(13, 3)]
        public decimal Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }

        public PartType PartType { get; set; }

        [StringLength(500, ErrorMessage = "Remark can't be longer than 500 characters")]
        public string? Remarks { get; set; }

        public string? Version { get; set; }

        public string? ScrapAllowance { get; set; }

        public string? ScrapAllowanceType { get; set; }
        public string? CustomFields { get; set; }
        public string? Designator { get; set; }
        public string? FootPrint { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        
        public List<EnggAlternatesUpdateDto>? EnggAlternatesUpdateDtos { get; set; }



    }
    public class EnggChildBomDetailsDto
    {
        public string? ItemNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? UOM { get; set; }
        public string? Description { get; set; }
        public PartType PartType { get; set; }
        public bool IsActive { get; set; }

    }
    public class EnggChildBomQtyDetailsDto
    {
        public string? ItemNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal Quantity { get; set; }

    }
}
