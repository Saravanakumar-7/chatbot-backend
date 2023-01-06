using Entities;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IDocumentUploadRepository : IRepositoryBase<DocumentUpload>
    {
        Task<int?> CreateUploadDocumentGrin(DocumentUpload documentUpload);
    }
}
