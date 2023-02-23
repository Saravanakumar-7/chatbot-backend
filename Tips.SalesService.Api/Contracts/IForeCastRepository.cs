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
        Task<PagedList<ForeCast>> GetAllForeCast(PagingParameter pagingParameter);
        Task<ForeCast> GetForeCastById(int id);
        Task<int?> CreateForeCast(ForeCast foreCast);
        Task<string> UpdateForeCast(ForeCast foreCast);
        Task<string> DeleteForeCast(ForeCast foreCast);
        Task<IEnumerable<ForeCastNumberListDto>> GetAllActiveForeCastNumberList();
        Task<ForeCast> ForeCastSourcingByForecasrNumbers(string id);
        Task<ForeCast> ForeCastLpcostingByForeCastNumbers(string id);
        Task<ForeCast> ForeCastLpCostingReleaseByForeCastNumbers(string id);
    }
}
