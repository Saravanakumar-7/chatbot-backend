using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IMaterialTransactionNoteItemRepository : IRepositoryBase<MaterialTransactionNoteItem>
    {
        Task<PagedList<MaterialTransactionNoteItem>> GetAllMaterialTransactionNoteItem(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<MaterialTransactionNoteItem> GetMaterialTransactionNoteItemById(int id);
        Task<int?> CreateMaterialTransactionNoteItem(MaterialTransactionNoteItem mtni);
        Task<string> UpdateMaterialTransactionNoteItem(MaterialTransactionNoteItem mtni);
        Task<string> DeleteMaterialTransactionNoteItem(MaterialTransactionNoteItem mtni);
        public void SaveAsync();
    }
}