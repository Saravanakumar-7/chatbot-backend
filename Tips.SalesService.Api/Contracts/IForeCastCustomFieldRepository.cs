using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCustomFieldRepository : IRepositoryBase<ForeCastCustomField>
    {
        Task<PagedList<ForeCastCustomField>> GetAllForeCastCustomField(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForeCastCustomField> GetForeCastCustomFieldById(int id);
        Task<int?> CreateForeCastCustomField(ForeCastCustomField foreCastCustomField);
        Task<string> UpdateForeCastCustomField(ForeCastCustomField foreCastCustomField);
        Task<string> DeleteForeCastCustomField(ForeCastCustomField foreCastCustomField);
    }
}
