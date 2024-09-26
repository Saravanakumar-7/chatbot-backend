using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorRelatedVendorRepository
    {
        Task<IEnumerable<VendorRelatedVendor>> GetAllVendorRelatedVendor();

        Task<VendorRelatedVendor> GetVendorRelatedVendorById(int id);

        Task<int?> CreateVendorRelatedVendor(VendorRelatedVendor vendorRelatedVendor);

        Task<string> UpdateVendorRelatedVendor(VendorRelatedVendor vendorRelatedVendor);

        Task<string> DeleteVendorRelatedVendor(VendorRelatedVendor vendorRelatedVendor);
    }
}
