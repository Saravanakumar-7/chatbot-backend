using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;

namespace Contracts
{
    public interface ICustomerMasterRepository:IRepositoryBase<CustomerMaster>
    {
        Task<IEnumerable<CustomerMaster>> GetAllCustomerMaster();
        Task<CustomerMaster> GetCustomerMasterById(int id);
        Task<IEnumerable<CustomerMaster>> GetAllActiveCustomerMaster();
        Task<int?> CreateCustomerMaster(CustomerMaster customerMaster);
        Task<string> UpdateCustomerMaster(CustomerMaster customerMaster);
        Task<string> DeleteCustomerMaster(CustomerMaster customerMaster);
        Task<IEnumerable<CustomerIdNameListDto>> GetAllActiveCustomerIdNameList();
    }
}
