using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        Task<PagedList<MaterialReturnNote>> GetAllMRNStatusOpen(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<IEnumerable<MaterialReturnNote>> GetAllMRNStatusClose();
        Task<IEnumerable<MaterialReturnNoteIdNameList>> GetAllMaterialReturnNoteIdNameList();
        Task<IEnumerable<MaterialReturnNote>> GetAllMaterialReturnNoteWithItems(MaterialReturnNoteSearchDto materialReturnNoteSearch);
        Task<IEnumerable<MaterialReturnNote>> SearchMaterialReturnNote([FromQuery] SearchParamess searchParammes);
        Task<IEnumerable<MaterialReturnNote>> SearchMaterialReturnNoteDate([FromQuery] SearchDateparames searchDatesParams);
        public void SaveAsync();
    }
}
