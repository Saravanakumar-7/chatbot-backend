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
    }
}
