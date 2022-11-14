using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ICustomerBankingsRepository : IRepositoryBase<CustomerBanking>
    {
        Task<IEnumerable<CustomerBanking>> GetAllCustomerBanking();
        Task<CustomerBanking> GetCustomerBankingById(int id);
        Task<IEnumerable<CustomerBanking>> GetAllActiveCustomerBanking();
        Task<int?> CreateCustomerBanking(CustomerBanking customerBanking);
        Task<string> UpdateCustomerBanking(CustomerBanking customerBanking);
        Task<string> DeleteCustomerBanking(CustomerBanking customerBanking);
    }
}
