using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReleaseProductBomDto
    {
        public int Id { get; set; }
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public PartType ItemType { get; set; }
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? Releasefileuploads { get; set; }

    }
    public class ReleaseProductBomDtoPost
    {
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public PartType ItemType { get; set; }
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }
        public string? Releasefileuploads { get; set; }

    }
    public class ReleaseProductBomDtoUpdate
    {
        public int Id { get; set; }
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public PartType ItemType { get; set; }
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }
        public string? Releasefileuploads { get; set; }

    }
    public class GetAllReleaseProductBomItemNumberVersionList
    {
        public string ItemNumber { set; get; }
        public decimal[] ReleaseVersion { get; set; }
        public string? ItemDescription { get; set; }

        public PartType ItemType { get; set; }
    }
    public class ProductionBomRevisionNumber
    {
        public string ItemNumber { get; set; }
        public List<string> FGItemNumber { get; set; }

        public PartType ItemType { get; set; }
        public decimal[] BomVersionNo { get; set; }

    }

    public class ProductionBomRevisionNumberAndQty
    {
        public string ItemNumber { get; set; }

        /// <summary>
        /// This is FG Itemnumber with Bom Qty of given SA present on this FG BOM
        /// </summary>
        public Dictionary<string,decimal> FGItemNumberWithSaBomQty { get; set; }
        public PartType ItemType { get; set; }
        public decimal[] BomVersionNo { get; set; }
    }

    public class GetBomQuantityDto
    { 
        public decimal BomQuantity { get; set; }

    }
    public class ReleaseProductionBomSPReport
    {
        public string? ItemNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ItemType { get; set; }
        public decimal? ReleaseVersion { get; set; }
    }
    public class ReleaseProductionBomSPReportDto
    {
        public string? ItemNumber { get; set; }
    }

    public class BomLatestRevisionDataSp
    {
        // enggboms
        public int? BOMId { get; set; }
        public string? BOMItemNumber { get; set; }
        public int? ItemType { get; set; }
        public decimal? RevisionNumber { get; set; }

        // enggchilditems
        public string? MftrItemNumbers { get; set; }
        public string? UOM { get; set; }
        public decimal? Quantity { get; set; }
        public string? Description { get; set; }
        public int? PartType { get; set; }
        public string? BOMChildItemNumbere { get; set; }
        public string? Remarks { get; set; }
        public int? EnggBomId { get; set; }
    }

    public class BomReportwithallRevisionsSp
    {
        // enggboms (Parent)
        public int? BOMId { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public int? ItemType { get; set; }
        public decimal? RevisionNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? Unit { get; set; }
        public bool? IsEnggBomRelease { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? Remarks { get; set; }

        // enggchilditems (Child)
        public string? ecItemNumber { get; set; }
        public string? MftrItemNumbers { get; set; }
        public string? UOM { get; set; }
        public decimal? Quantity { get; set; }
        public string? Description { get; set; }
        public int? PartType { get; set; }
        public string? ecRemarks { get; set; }
        public string? Version { get; set; }
        public string? ScrapAllowance { get; set; }
        public string? ScrapAllowanceType { get; set; }
        public string? CustomFields { get; set; }
        public bool? ecIsActive { get; set; }
        public int? EnggBomId { get; set; }
        public string? Designator { get; set; }
        public string? FootPrint { get; set; }
    }

    public class BomReportwithallRevisionsSpInput
    {
        public string? ItemNumber { get; set; }
        public decimal? RevisionNumber { get; set; }
    }

    }
