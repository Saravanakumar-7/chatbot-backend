using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CostingBomDto
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
    public class CostingBomDtoPost
    {
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public PartType ItemType { get; set; }
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }
        public string? Releasefileuploads { get; set; }

    }
    public class CostingBomDtoUpdate
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
        public class CostingBomItemRevisionList
    {
        public string ItemNumber { set; get; }
        public decimal[] ReleaseVersion { get; set; }
        public string? ItemDescription { get; set; }

        public PartType ItemType { get; set; }

    }
    public class CostingBomRevisionNumberList
    {
        public decimal[] ReleaseVersion { get; set; }

    }
    public class ReleasedCostingFGTGItemNoListDto
    {
        public long id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public PartType PartType { get; set; }
    }
}
