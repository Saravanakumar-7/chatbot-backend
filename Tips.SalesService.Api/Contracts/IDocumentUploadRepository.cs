using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IDocumentUploadRepository : IRepositoryBase<DocumentUpload>
    {
        Task<int?> CreateUploadDocument(DocumentUpload documentUpload);
        Task<List<DocumentUploadDto>> GetDownloadUrlDetails(string FileIds);
    }
}