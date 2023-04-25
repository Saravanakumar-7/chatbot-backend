using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface ICustomerMasterRepository:IRepositoryBase<CustomerMaster>
    {
        Task<PagedList<CustomerMaster>> GetAllCustomerMasters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CustomerMaster> GetCustomerMasterById(int id);
        Task<IEnumerable<CustomerMaster>> GetAllActiveCustomerMasters();
        Task<int?> CreateCustomerMaster(CustomerMaster customerMaster);
        Task<string> UpdateCustomerMaster(CustomerMaster customerMaster);
        Task<string> DeleteCustomerMaster(CustomerMaster customerMaster);
        Task<IEnumerable<CustomerIdNameListDto>> GetAllActiveCustomerMasterIdNameList();
        Task<IEnumerable<CustomerIdNameListDto>> GetAllCustomerMasterIdNameList();
        Task<CustomerMaster> GetCSNumberAutoIncrementCount();
        Task<CustomerMaster> GetCustomerMasterByCustomerNo(string customerNumber);

    }
}
