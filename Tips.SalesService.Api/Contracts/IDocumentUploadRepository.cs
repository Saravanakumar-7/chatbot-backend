using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IDocumentUploadRepository : IRepositoryBase<DocumentUpload>
    {
        Task<int?> CreateUploadDocument(DocumentUpload documentUpload);
    }
}