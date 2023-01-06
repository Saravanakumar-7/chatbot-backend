using Entities;
using Entities.DTOs;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVendorRepository : IRepositoryBase<VendorMaster>
    {
        Task<PagedList<VendorMaster>> GetAllVendorMasters(PagingParameter pagingParameter);
        Task<VendorMaster> GetVendorMasterById(int id);
        Task<IEnumerable<VendorMaster>> GetAllActiveVendorMasters();
        Task<int?> CreateVendorMaster(VendorMaster vendorMaster);
        Task<string> UpdateVendorMaster(VendorMaster vendorMaster);
        Task<string> DeleteVendorMaster(VendorMaster vendorMaster);
        Task<IEnumerable<VendorIdNameListDto>> GetAllActiveVendorMasterNameList();

    }
}
