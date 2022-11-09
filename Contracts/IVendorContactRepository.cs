using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorContactRepository
    {
        Task<IEnumerable<VendorContacts>> GetAllVendorsContact();
        Task<VendorContacts> GetVendorContactById(int id);
        Task<IEnumerable<VendorContacts>> GetAllActiveVendorsContact();
        Task<int?> CreateVendorContact(VendorContacts vendorContacts);
        Task<string> UpdateVendorcontact(VendorContacts vendorContacts);
        Task<string> DeleteVendorContact(VendorContacts vendorContacts);
    }
}
