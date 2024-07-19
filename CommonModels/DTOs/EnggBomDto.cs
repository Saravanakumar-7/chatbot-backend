using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
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

        [Precision(5, 2)]
        public decimal? RevisionNumber { get; set; }
        public string? Remarks { get; set; }

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
        public string? Remarks { get; set; }

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
        public string? ItemDescription { set; get; }

    }
    public class EnggBomRevisionNumberList
    {
        public decimal[] RevisionNumber { get; set; }

    }

    public class EnggBomFGItemNumber
    {
        public string ItemNumber { set; get; }
        public string? Description { set; get; }

    }
    public class EnggBomFGItemNumberWithQtyDto
    {
        public string ItemNumber { set; get; }
        public string? ItemDescription { set; get; }
        public decimal QtyReq { set; get; }

    }
    public class EnggBomFGCostItemNumberWithQtyDto
    {
        public string? FGItemNumber { set; get; }
        public string? ItemNumber { set; get; }
        public string? ItemDescription { set; get; }
        public decimal QtyReq { set; get; }
        public decimal WeightedAvg { set; get; }

    }
    public class EnggBomItemDto
    {
        public string ItemNumber { get; set; }

    }
    public class EnggBomCoverageDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public decimal? BalanceForcastQty { get; set; }
    }
    public class CoverageEnggChildDto
    {
        //public string ItemNumber { get; set; }
        //public string ChildNumber { get; set; }
        //public string Description { get; set; }
        //public string ChildDescription { get; set; }
        //public PartType PartType { get; set; }        
        //public decimal QtyPerUnit { get; set; }
        //public decimal? BalanceForcastQty { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public decimal? BalanceForcastQty { get; set; }


    }
    public class RfqSourcingPPdetailsforEngg
    {
        public string PPItemNumber { get; set; }
        public decimal? VLandindPrice { get; set; }
        public decimal? VMoqcost { get; set; }
    }
    public class FGFinalLandedandMoqPrice
    {
        public string FGItemNumber { get; set; }
        public decimal? FinalLandindPrice { get; set; }
        public decimal? FinalMoqcost { get; set; }
    }
    public class SAFinalLandedandMoqPrice
    {
        public decimal? SAFinalLandindPrice { get; set; }
        public decimal? SAFinalMoqcost { get; set; }
    }
    public class RfqEnggitemSourcingDto
    {
        public string? ItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal? CostingBomVersionNo { get; set; }

    }
    public class FGItemNumberListDto
    {
        public string? FGItemNumber { get; set; }
    }
    public class EnggBomSPReportDto
    {
        public int? BOMId { get; set; }
    }
    public class EnggBomDetailsDto
    {
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public PartType ItemType { get; set; }
        public bool IsActive { get; set; }

    }
    public class FGCostingSPReport
    {
        public string? FGItemnumber { get; set; }
        public string? FGChildItemNumber { get; set; }
        public string? DrawingRevNo { get; set; }
        public string? Description { get; set; }
        public string? MftrItemNumbers { get; set; }
        public decimal? Qntyper { get; set; }
        public string? UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Totalamount { get; set; }
    }
    public class FGCostingSPReportDto
    {
        public string? FGItemnumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        
    }


}
