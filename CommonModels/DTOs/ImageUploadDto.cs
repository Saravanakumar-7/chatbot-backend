using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ImageUploadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string? FileByte { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class ImageUploadPostDto
    {
        [RegularExpression(@"^[a-zA-Z0-9_ ]+$", ErrorMessage = "FileName can only contain letters, numbers, underscores, and spaces.")]
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FileByte { get; set; }

    }
    public class ImageUploadUpdateDto
    {
        public int? Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public string? FileByte { get; set; }

    }
}
