using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ItemMasterFileUploadDto
    {
        [Key]
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
        public string? UploadedBy { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ItemMasterFileUploadDtoPost
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
        public string? UploadedBy { get; set; }
      
    }

    public class ItemMasterFileUploadDtoUpdate
    {
        [Key]
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
        public string? UploadedBy { get; set; }
 
    }
}
