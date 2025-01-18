using Google.Cloud.Storage.V1;
using Tips.SalesService.Api.Contracts;

namespace Tips.SalesService.Api.Repository
{
    public class GoogleGCPstorageService : IGoogleGCPstorageService
    {
        private readonly StorageClient _storageClient;
       // private readonly string _bucketName;

        public GoogleGCPstorageService()//string bucketName
        {
            _storageClient = StorageClient.Create();
           // _bucketName = bucketName;
        }

        public async Task UploadFileAsync(string localFilePath, string filename, string _bucketName)
        {
            using var fileStream = File.OpenRead(localFilePath);
            await _storageClient.UploadObjectAsync(_bucketName, filename, null, fileStream);
            Console.WriteLine($"File {filename} uploaded to bucket {_bucketName}.");
        }
        public string GenerateSignedUrl(string filename, TimeSpan duration, string _bucketName)
        {
            UrlSigner urlSigner = UrlSigner.FromServiceAccountPath("path-to-service-account-key.json");
            string signedUrl = urlSigner.Sign(_bucketName, filename, duration);
            return signedUrl;
        }
    }
}
