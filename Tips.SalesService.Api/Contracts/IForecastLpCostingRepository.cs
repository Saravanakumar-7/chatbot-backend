using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForecastLpCostingRepository : IRepositoryBase<ForecastLpCosting>
    {
        Task<PagedList<ForecastLpCosting>> GetAllForecastLpCosting(PagingParameter pagingParameter);
        Task<ForecastLpCosting> GetForecastLpCostingById(int id);
        Task<int?> CreateForecastLpCosting(ForecastLpCosting forecastLpCosting);
        Task<string> UpdateForecastLpCosting(ForecastLpCosting forecastLpCosting);
        Task<string> DeleteForecastLpCosting(ForecastLpCosting forecastLpCosting);
        Task<ForecastLpCosting> GetForecastLpCostingByForeCastNumber(string ForeCastNumber);
    }
}
