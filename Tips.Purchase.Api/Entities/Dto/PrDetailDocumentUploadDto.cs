using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrDetailDocumentUploadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FilePath { get; set; } 
        }
    public class PrDetailDocumentUploadPostDto
    { 
        public string FileName { get; set; }

        public string FileExtension { get; set; }
        public string? FileByte { get; set; }

        //public string FilePath { get; set; } 
    }
    public class PrDetailDocumentUploadUpdateDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string? FileByte { get; set; }
    }
    public class PrDetailDocumentApprovalUploadUpdateDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string? FileByte { get; set; }
    }


}
