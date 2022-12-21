using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IMaterialTransactionNoteRepository : IRepositoryBase<MaterialTransactionNote>
    {
        Task<PagedList<MaterialTransactionNote>> GetAllMaterialTransactionNote(PagingParameter pagingParameter);
        Task<MaterialTransactionNote> GetMaterialTransactionNoteById(int id);
        Task<int?> CreateMaterialTransactionNote(MaterialTransactionNote mtn);
        Task<string> UpdateMaterialTransactionNote(MaterialTransactionNote mtn);
        Task<string> DeleteMaterialTransactionNote(MaterialTransactionNote mtn);
        public void SaveAsync();
    }
}