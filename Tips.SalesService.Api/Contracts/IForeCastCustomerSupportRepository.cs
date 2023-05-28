using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCustomerSupportRepository : IRepositoryBase<ForeCastCustomerSupport>
    {
        Task<PagedList<ForeCastCustomerSupport>> GetAllForeCastCustomerSupports(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForeCastCustomerSupport> GetForeCastCustomerSupportById(int id);
        Task<int?> CreateForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport);
        Task<string> UpdateForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport);
        Task<string> DeleteForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport);
        Task<ForeCastCustomerSupport> GetForeCastCustomerSupportByForeCastNumber(string ForeCastNumber);

        // here added new apis

        Task<ForeCastCustomerSupport> UpdateForecastcsRevNo(ForeCastCustomerSupport foreCastCustomerSupport);
        Task<ForeCastCustomerSupport> GetForecastCsByForecastNoAndRevNo(string forecast, decimal revisionNumber);
        Task<ForeCastCustomerSupport> GetForecastCsLatestRevNoByForecastnumber(string forecast);

    }
}
