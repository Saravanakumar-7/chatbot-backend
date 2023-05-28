using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;
using Entities.DTOs;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastRepository : IRepositoryBase<ForeCast>
    {
        Task<PagedList<ForeCast>> GetAllForeCast(PagingParameter pagingParameter, SearchParammes searchParammes);
       Task<ForeCast> GetForeCastById(int id);
        Task<int?> CreateForeCast(ForeCast foreCast);
        Task<string> UpdateForeCast(ForeCast foreCast);
        Task<string> DeleteForeCast(ForeCast foreCast);
        Task<IEnumerable<ForeCastNumberListDto>> GetAllActiveForeCastNumberList();
        Task<IEnumerable<LatestForecastNumberListDto>> GetAllActiveLatestForeCastNumbers();


        Task<ForeCast> ForeCastCustomerSupportByForeCastNumber(string ForeCastNumber);

        Task<ForeCast> ForeCastSourcingByForecasrNumbers(string id);
        Task<ForeCast> ForeCastLpcostingByForeCastNumbers(string id);
        Task<ForeCast> ForeCastLpCostingReleaseByForeCastNumbers(string id);

        //Task<int?> GetForecastNumberAutoIncrementCount(DateTime date);
        //Task<string> GetForecastNumberAutoIncrementNumber();
        Task<string> GenerateForecastNumber();
        Task<IEnumerable<ForeCastNumberListDto>> GetAllForecastNumberList();
        Task<IEnumerable<ForeCastNumberListDto>> GetAllActiveForecastNumberListByCustomerId(string CustomerId);
        Task<ForeCast> ForecastDetailsByForecastNumbers(string forecast);
        Task<ForeCast> GetCustomerIdByForecastNumber(string forecast);
        Task<IEnumerable<RevNumberByForecastNumberListDto>> GetRevNumberByForecastNumberList(string forecast);

        Task<ForeCast> GetForecastDeatailsByForecastNoAndRevNo(string forecast, int revisionNumber);

        Task<ForeCast> UpdateForecastRevNo(ForeCast forecast);



    }
}
