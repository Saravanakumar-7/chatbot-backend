using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerMasterOtherUploadsRepository:IRepositoryBase<CustomerOtherUploads>
    {
        Task<int?> CreateCustomerOtherUploads(CustomerOtherUploads customerOtherUploads);
        Task<CustomerOtherUploads> GetCustomerMasterOtherUploadsbyCustomerId(int Id);
        Task<string> UpdateCustomerOtherUploads(CustomerOtherUploads customerOtherUploads);
    }
}
