using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialReturnNoteRepository : IRepositoryBase<MaterialReturnNote>
    {
        Task<PagedList<MaterialReturnNote>> GetAllMaterialReturnNote(PagingParameter pagingParameter);
        Task<MaterialReturnNote> GetMaterialReturnNoteById(int id);
       
        Task<int?> CreateMaterialReturnNote(MaterialReturnNote materialReturnNote);
        Task<string> UpdateMaterialReturnNote(MaterialReturnNote materialReturnNote);
        Task<string> DeleteMaterialReturnNote(MaterialReturnNote materialReturnNote);

        public void SaveAsync();
    }
}
