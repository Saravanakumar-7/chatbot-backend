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
        Task<IEnumerable<CustomerBanking>> GetAllCustomerBankings();
        Task<CustomerBanking> GetCustomerBankingById(int id);
        Task<IEnumerable<CustomerBanking>> GetAllActiveCustomerBankings();
        Task<int?> CreateCustomerBanking(CustomerBanking customerBanking);
        Task<string> UpdateCustomerBanking(CustomerBanking customerBanking);
        Task<string> DeleteCustomerBanking(CustomerBanking customerBanking);
    }
}
