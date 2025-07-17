namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SalesServiceFileUploadDto
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
    public class SalesServiceFileUploadPostDto
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FileByte { get; set; }
    }
}
