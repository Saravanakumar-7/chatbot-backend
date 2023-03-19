using Entities;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IDocumentUploadRepository : IRepositoryBase<DocumentUpload>
    {
        Task<int?> CreateUploadDocumentGrin(DocumentUpload documentUpload);
        
        Task<IEnumerable<GetDownloadUrlDto>> GetGrinDownloadUrlDetails(string grinNumber);

        Task<IEnumerable<GetDownloadUrlDto>> GetGrinPartsDownloadUrlDetails(string grinNumber);
        Task<int?> GetDocumentDetailsByGrinNo(string grinnumber);
        Task<string> DeleteGrinPartsUploadDocByGrinNo(string grinnumber);
        Task<DocumentUpload> GetUploadDocById(int id);
        Task<string> DeleteUploadFile(DocumentUpload documentUpload); 
    }
}
