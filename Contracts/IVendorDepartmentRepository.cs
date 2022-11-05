using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorDepartmentRepository : IRepositoryBase<VendorDepartment>
    {
        Task<IEnumerable<VendorDepartment>> GetAllVendorDepartment();
        Task<VendorDepartment> GetVendorDepartmentById(int id);
        Task<IEnumerable<VendorDepartment>> GetAllActiveVendorDepartment();
        Task<int?> CreateVendorDepartment(VendorDepartment vendorDepartment);
        Task<string> UpdateVendorDepartment(VendorDepartment vendorDepartment);
        Task<string> DeleteVendorDepartment(VendorDepartment vendorDepartment);

    }
}
