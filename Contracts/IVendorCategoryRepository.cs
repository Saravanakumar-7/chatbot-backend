using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorCategoryRepository : IRepositoryBase<VendorCategory>
    {
        Task<IEnumerable<VendorCategory>> GetAllVendorCategory();
        Task<VendorCategory> GetVendorCategoryById(int id);
        Task<IEnumerable<VendorCategory>> GetAllActiveVendorCategory();
        Task<int?> CreateVendorCategory(VendorCategory vendorCategory);
        Task<string> UpdateVendorCategory(VendorCategory vendorCategory);
        Task<string> DeleteVendorCategory(VendorCategory vendorCategory);

    }
}
