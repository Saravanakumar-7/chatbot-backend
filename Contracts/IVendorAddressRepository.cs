using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorAddressRepository
    {
        Task<IEnumerable<VendorAddress>> GetAllVendorsAddress();
        Task<VendorAddress> GetVendorAddressById(int id);
        Task<IEnumerable<VendorAddress>> GetAllActiveVendorsAddress();
        Task<int?> CreateVendorAddress(VendorAddress vendorAddress);
        Task<string> UpdateVendorAddress(VendorAddress vendorAddress);
        Task<string> DeleteVendorAddress(VendorAddress vendorAddress);
    }
}
