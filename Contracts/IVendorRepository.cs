using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorRepository : IRepositoryBase<VendorMaster>
    {
        Task<IEnumerable<VendorMaster>> GetAllVendors();
        Task<VendorMaster> GetVendorById(int id);
        Task<IEnumerable<VendorMaster>> GetAllActiveVendors();
        Task<int?> CreateVendor(VendorMaster vendorMaster);
        Task<string> UpdateVendor(VendorMaster vendorMaster);
        Task<string> DeleteVendor(VendorMaster vendorMaster);
    }
}
