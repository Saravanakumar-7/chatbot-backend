using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PRItemsDocumentUploadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string FileByte { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class PRItemsDocumentUploadPostDto
    {
        [RegularExpression(@"^[a-zA-Z0-9_.\- ]+$", ErrorMessage = "FileName can only contain letters, numbers, underscores (_), hyphens (-), FullStop (.) and spaces.")]
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FileByte { get; set; }
        public bool NewFile { get; set; }

    }


    public class PRItemsDocumentUploadDocsDto
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FilePath { get; set; }
        public string FileByte { get; set; }

        public string DocumentFrom { get; set; }

        public string ParentNumber { get; set; }
        public bool Checked { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PrItemId { get; set; }
        public string? FileUrl { get; set; }

    }
    public class PRItemsDocumentUploadUpdateDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public byte[] FileByte { get; set; }

    }

    public class PRItemsGetDownloadUrlDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string DownloadUrl { get; set; }
    }
    public class GetPRItemsDownloadUrlDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string DownloadUrl { get; set; }
    }
}