using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class ReturnGrinDocumentUpload
    {
        [Key]
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FilePath { get; set; }

        public string DocumentFrom { get; set; }

        public string ParentId { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
