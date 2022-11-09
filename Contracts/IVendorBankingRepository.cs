using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorBankingRepository
    {
        Task<IEnumerable<VendorBanking>> GetAllVendorsBanking();
        Task<VendorBanking> GetVendorBankingById(int id);
        Task<IEnumerable<VendorBanking>> GetAllActiveVendorsBanking();
        Task<int?> CreateVendorBanking(VendorBanking vendorBanking);
        Task<string> UpdateVendorBanking(VendorBanking vendorBanking);
        Task<string> DeleteVendorBanking(VendorBanking vendorBanking);
    }
}
