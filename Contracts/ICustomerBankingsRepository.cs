using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICustomerBankingsRepository : IRepositoryBase<CustomerBanking>
    {
        Task<PagedList<CustomerBanking>> GetAllCustomerBankings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CustomerBanking> GetCustomerBankingById(int id);
        Task<PagedList<CustomerBanking>> GetAllActiveCustomerBankings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCustomerBanking(CustomerBanking customerBanking);
        Task<string> UpdateCustomerBanking(CustomerBanking customerBanking);
        Task<string> DeleteCustomerBanking(CustomerBanking customerBanking);
    }
}
