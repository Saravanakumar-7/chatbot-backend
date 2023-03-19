using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialReturnNoteItemRepository : IRepositoryBase<MaterialReturnNoteItem>
    {

        Task<PagedList<MaterialReturnNoteItem>> GetAllMaterialReturnNoteItems(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<MaterialReturnNoteItem> GetMaterialReturnNoteItemById(int id);

        Task<int?> CreateMaterialReturnNoteItem(MaterialReturnNoteItem materialReturnNoteItem);
        Task<string> UpdateMaterialReturnNoteItem(MaterialReturnNoteItem materialReturnNoteItem);
        Task<string> DeleteMaterialReturnNoteItem(MaterialReturnNoteItem materialReturnNoteItem);

        public void SaveAsync();
    }
}
