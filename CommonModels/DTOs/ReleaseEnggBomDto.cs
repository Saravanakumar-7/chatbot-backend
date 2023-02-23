using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReleaseEnggBomDto
    {
        public int Id { get; set; }
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string ReleaseVersion { get; set; }
        public string ReleaseTypeMinor { get; set; }
        public string ReleaseTypeMajor { get; set; }
        public string ReleaseNote { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ReleaseEnggBomDtoPost
    {

        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string ReleaseVersion { get; set; }
        public string ReleaseTypeMinor { get; set; }
        public string ReleaseTypeMajor { get; set; }
        public string ReleaseNote { get; set; }

    }
    public class ReleaseEnggBomDtoUpdate
    {
        public int Id { get; set; }
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string ReleaseVersion { get; set; }
        public string ReleaseTypeMinor { get; set; }
        public string ReleaseTypeMajor { get; set; }
        public string ReleaseNote { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
