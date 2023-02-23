using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastEnggRepository : IRepositoryBase<ForeCastEngg>
    {
        Task<PagedList<ForeCastEngg>> GetAllForeCastEngg(PagingParameter pagingParameter);
        Task<ForeCastEngg> GetForeCastEnggById(int id);
        Task<int?> CreateForeCastEngg(ForeCastEngg foreCastEngg);
        Task<string> UpdateForeCastEngg(ForeCastEngg foreCastEngg);
        Task<string> DeleteForeCastEngg(ForeCastEngg foreCastEngg);
        Task<ForeCastEngg> ForeCastEnggByForeCastNumber(string ForeCastNumber);
    }
}
