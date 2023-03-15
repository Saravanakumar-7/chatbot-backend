using Entities;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IDocumentUploadRepository : IRepositoryBase<DocumentUpload>
    {
        Task<int?> CreateUploadDocumentPO(DocumentUpload documentUpload);
        Task<DocumentUpload> GetUploadDocById(int id);
        Task<string> DeleteUploadFile(DocumentUpload documentUpload);


    }
}
