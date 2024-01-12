using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorMasterOtherUploadsRepository : IRepositoryBase<VendorOtherUploads>
    {
        Task<int?> CreateVendorOtherUploads(VendorOtherUploads vendorOtherUploads);
        Task<VendorOtherUploads> GetVendorMasterOtherUploadsbyVendorId(int Id);
        Task<string> UpdateVendorOtherUploads(VendorOtherUploads vendorOtherUploads);
    }
}
