using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesServiceFileUploadRepository : IRepositoryBase<SalesServiceFileUpload>
    {
        Task<int?> CreateSalesServiceFileUploadDocument(SalesServiceFileUpload fileUpload);
        Task<List<SalesServiceFileUploadDto>> GetDownloadUrlDetails(string FileIds);
    }
}
