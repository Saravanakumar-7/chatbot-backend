using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialReturnNoteRepository : IRepositoryBase<MaterialReturnNote>
    {
        Task<PagedList<MaterialReturnNote>> GetAllMaterialReturnNotes(PagingParameter pagingParameter,SearchParamess searchParamess);
        Task<MaterialReturnNote> GetMaterialReturnNoteById(int id);
        Task<int?> GetMRNumberAutoIncrementCount(DateTime date);
        Task<int?> CreateMaterialReturnNote(MaterialReturnNote materialReturnNote);
        Task<string> UpdateMaterialReturnNote(MaterialReturnNote materialReturnNote);
        Task<string> DeleteMaterialReturnNote(MaterialReturnNote materialReturnNote);

        Task<IEnumerable<MaterialReturnNoteIdNameList>> GetAllMaterialReturnNoteIdNameList();

        public void SaveAsync();
    }
}
