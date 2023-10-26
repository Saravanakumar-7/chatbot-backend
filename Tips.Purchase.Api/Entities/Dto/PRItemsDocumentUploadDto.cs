namespace Tips.Purchase.Api.Entities.Dto
{
    public class PRItemsDocumentUploadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class PRItemsDocumentUploadPostDto
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileByte { get; set; }
        public bool NewFile { get; set; }

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