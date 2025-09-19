using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class FileUploadDto
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
    public class FileUploadPostDto
    {
        [RegularExpression(@"^[a-zA-Z0-9_.\- ]+$", ErrorMessage = "FileName can only contain letters, numbers, underscores (_), hyphens (-), FullStop (.) and spaces.")]
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FileByte { get; set; }

    }
    public class FileUploadUpdateDto
    {
        public int? Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public string? FileByte { get; set; }

    }
}
