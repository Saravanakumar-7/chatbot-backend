using Entities;
using Entities.Helper;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqCustomFieldRepository : IRepositoryBase<RfqCustomField>
    {
        Task<PagedList<RfqCustomField>> GetAllRfqCustomField(PagingParameter pagingParameter);
        Task<RfqCustomField> GetRfqCustomFieldById(int id);
        Task<int?> CreateRfqCustomField(RfqCustomField rfqCustomField);
        Task<string> UpdateRfqCustomField(RfqCustomField rfqCustomField);
        Task<string> DeleteRfqCustomField(RfqCustomField rfqCustomField);
    }
}
