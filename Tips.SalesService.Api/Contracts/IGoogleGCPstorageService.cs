using Tips.SalesService.Api.Repository;

namespace Tips.SalesService.Api.Contracts
{
    public interface IGoogleGCPstorageService
    {
        Task UploadFileAsync(string localFilePath, string filename, string _bucketName);
        string GenerateSignedUrl(string objectName, TimeSpan duration, string _bucketName);
    }
}
