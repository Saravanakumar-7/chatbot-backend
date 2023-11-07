using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class DocumentUploadDto
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
    public class DocumentUploadPostDto
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string? FileByte { get; set; }

    }
    public class DocumentUploadUpdateDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public byte[] FileByte { get; set; }

    }

    public class GetDownloadUrlDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string DownloadUrl { get; set; }
    }
    public class GetPRDownloadUrlDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public string DownloadUrl { get; set; }
    }
}
