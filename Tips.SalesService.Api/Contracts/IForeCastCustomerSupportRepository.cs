using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCustomerSupportRepository : IRepositoryBase<ForeCastCustomerSupport>
    {
        Task<PagedList<ForeCastCustomerSupport>> GetAllForeCastCustomerSupports(PagingParameter pagingParameter);
        Task<ForeCastCustomerSupport> GetForeCastCustomerSupportById(int id);
        Task<int?> CreateForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport);
        Task<string> UpdateForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport);
        Task<string> DeleteForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport);
        Task<ForeCastCustomerSupport> ForeCastCustomerSupportByForeCastNumber(string ForeCastNumber);
    }
}
