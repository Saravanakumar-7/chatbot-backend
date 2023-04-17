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
    public class EnggBomDto
    {
        public int BOMId { get; set; }

        public string ItemNumber { get; set; }

        public string? ItemDescription { get; set; }

        public PartType ItemType { get; set; }

        [Precision(5,2)]
        public decimal? RevisionNumber { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggChildItemDto>? EnggChildItemDtos { get; set; }
        public List<BomNREConsumableDto>? BomNREConsumableDto { get; set; }


    }
    public class EnggBomPostDto
    {
        [Required(ErrorMessage = "ItemNumber is required")]
        public string ItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }

        public PartType ItemType { get; set; }


        [DefaultValue(true)]
        public bool IsActive { get; set; }

    

        public List<EnggChildItemPostDto>? EnggChildItemPosts { get; set; }

        public List<BomNREConsumablePostDto>? BomNREConsumablePostDto { get; set; }



    }
    public class EnggBomUpdateDto
    { 

        [Required(ErrorMessage = "ItemNumber is required")]
        public string ItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "ItemDescription can't be longer than 500 characters")]
        public string? ItemDescription { get; set; }

        public PartType ItemType { get; set; }


        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggChildItemUpdateDto>? EnggChildItemUpdates { get; set; }

        public List<BomNREConsumableUpdateDto>? BomNREConsumableUpdateDto { get; set; }


    }
    public class ItemMasterNREDetailsListDto
    {
        public int BOMId { get; set; }

 
    }
    public enum RevisionType
    {
        minor,
        major
    }

    
 public class EnggBomItemRevisionList
    {
        public string ItemNumber { set; get; }
        public decimal[] RevisionNumber { get; set; }

    }
    public class EnggBomRevisionNumberList
    {
        public decimal[] RevisionNumber { get; set; }

    }

    public class EnggBomFGItemNumber
    {
        public string ItemNumber { set; get; }
        public string ItemDescription { set; get; }

    }
    public class EnggBomFGItemNumberWithQtyDto
    {
        public string ItemNumber { set; get; }
        public string ItemDescription { set; get; }
        public decimal QtyReq { set; get; }

    }
    public class EnggBomItemDto
    {
        public string ItemNumber { get; set; }

    }
}
