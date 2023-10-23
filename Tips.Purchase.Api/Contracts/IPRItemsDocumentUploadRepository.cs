using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPRItemsDocumentUploadRepository : IRepositoryBase<PRItemsDocumentUpload>
    {
        Task<int?> CreateUploadDocumentPO(PRItemsDocumentUpload documentUpload);
        Task<PRItemsDocumentUpload> GetUploadDocById(int id);
        Task<string> DeleteUploadFile(PRItemsDocumentUpload documentUpload);
    }
}