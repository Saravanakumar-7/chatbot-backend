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
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class ReleaseProductBomDtoPost
    {
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }

    }
    public class ReleaseProductBomDtoUpdate
    {
        public int Id { get; set; }
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; } 

    }
    public class GetAllReleaseProductBomItemNumberVersionList
    {
        public string ItemNumber { set; get; }
        public decimal[] ReleaseVersion { get; set; }

    }
    public class ProductionBomRevisionNumberList
    {
        public decimal[] ReleaseVersion { get; set; }

    }
}
