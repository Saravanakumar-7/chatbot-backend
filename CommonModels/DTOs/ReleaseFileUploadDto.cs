using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReleaseFileUploadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FilePath { get; set; }
        public string? DownloadUrl { get; set; }
        public string? FileByte { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class ReleaseFileUploadPostDto
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FileByte { get; set; }
        public string? ReleaseFor { get; set; }


    }
    public class ReleaseFileUploadUpdateDto
    {
        public int? Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FileByte { get; set; }

    }
}
