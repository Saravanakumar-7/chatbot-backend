using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EnggBomDto
    {
        public int Id { get; set; }

        public string ItemNumber { get; set; }

        public string? ItemDescription { get; set; }

        public string? ItemType { get; set; }

        public string? RevisionNumber { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggChildItemDto>? EnggChildItemDtos { get; set; }

    }
    public class EnggBomPostDto
    {
        [Required(ErrorMessage = "ItemNumber is required")]
        public string ItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }

        public string? ItemType { get; set; }

        public string? RevisionNumber { get; set; }
        
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggChildItemPostDto>? EnggChildItemPosts { get; set; }


    }
    public class EnggBomUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ItemNumber is required")]
        public string ItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }

        public string? ItemType { get; set; }

        public string? RevisionNumber { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggChildItemUpdateDto>? EnggChildItemUpdates { get; set; }

    }

}
