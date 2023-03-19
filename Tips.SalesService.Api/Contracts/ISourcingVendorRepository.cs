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
    public interface ISourcingVendorRepository
    {
        Task<PagedList<SourcingVendor>> GetAllSourcingVendor(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<SourcingVendor> GetSourcingVendorById(int id);
        Task<int?> CreateSourcingVendor(SourcingVendor sourcingVendor);
        Task<string> UpdateSourcingVendor(SourcingVendor sourcingVendor);
        Task<string> DeleteSourcingVendor(SourcingVendor sourcingVendor);
    }
}
