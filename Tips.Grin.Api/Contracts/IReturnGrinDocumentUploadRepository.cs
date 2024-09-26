using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Repository;

namespace Tips.Grin.Api.Contracts
{
    public interface IReturnGrinDocumentUploadRepository : IRepositoryBase<ReturnGrinDocumentUpload>
    {
        Task<int?> CreateReturnGrinDocumentUpload(ReturnGrinDocumentUpload returnGrinDocumentUpload);
    }
}
