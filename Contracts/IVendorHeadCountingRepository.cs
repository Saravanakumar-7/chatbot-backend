using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorHeadCountingRepository
    {
        Task<IEnumerable<VendorHeadCounting>> GetAllVendorHeadCountings();
        Task<VendorHeadCounting> GetVendorHeadCountingById(int id);
        Task<IEnumerable<VendorHeadCounting>> GetAllActiveVendorHeadCountings();
        Task<int?> CreateVendorHeadCounting(VendorHeadCounting headCounting);
        Task<string> UpdateVendorHeadCounting(VendorHeadCounting headCounting);
        Task<string> DeleteVendorHeadCounting(VendorHeadCounting headCounting);

    }
}
