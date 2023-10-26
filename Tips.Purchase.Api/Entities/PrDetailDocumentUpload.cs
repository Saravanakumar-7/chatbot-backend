using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PrDetailDocumentUpload
    {

        [Key]
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FilePath { get; set; }

        public string DocumentFrom { get; set; }

        public string ParentNumber { get; set; }
        public bool Checked { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PrDetailsId { get; set; }
        public PrDetails? PrDetails { get; set; }
    }
}
