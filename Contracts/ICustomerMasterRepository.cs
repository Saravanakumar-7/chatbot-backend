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
        Task<PagedList<CustomerMaster>> GetAllActiveCustomerMasters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCustomerMaster(CustomerMaster customerMaster);
        Task<string> UpdateCustomerMaster(CustomerMaster customerMaster);
        Task<string> DeleteCustomerMaster(CustomerMaster customerMaster);
        Task<IEnumerable<CustomerIdNameListDto>> GetAllActiveCustomerMasterIdNameList();
        
        Task<CustomerMaster> GetCSNumberAutoIncrementCount();
        Task<CustomerMaster> GetCustomerMasterByCustomerNo(string customerNumber);

    }
}
