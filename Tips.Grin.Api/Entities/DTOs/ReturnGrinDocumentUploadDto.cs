namespace Tips.Grin.Api.Entities.DTOs
{
    public class ReturnGrinDocumentUploadDto
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
    public class ReturnGrinDocumentUploadDtoPost
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileByte { get; set; }

    }
    //public class ReturnGrinDocumentUploadDtoUpdate
    //{
    //    public int Id { get; set; }
    //    public string FileName { get; set; }
    //    public string FileExtension { get; set; }
    //    public byte[] FileByte { get; set; }
    //}
}
