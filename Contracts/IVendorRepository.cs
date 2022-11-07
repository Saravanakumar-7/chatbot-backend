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
        Task<IEnumerable<VendorMaster>> GetAllVendorMaster();
        Task<VendorMaster> GetVendorMasterById(int id);
        Task<IEnumerable<VendorMaster>> GetAllActiveVendorMaster();
        Task<int?> CreateVendorMaster(VendorMaster vendorMaster);
        Task<string> UpdateVendorMaster(VendorMaster vendorMaster);
        Task<string> DeleteVendorMaster(VendorMaster vendorMaster);
    }
 

    //public interface IVendorRepository : IRepositoryBase<VendorMasterBanking>
    //{
    //    Task<IEnumerable<VendorMasterBanking>> GetAllVendorMasterBanking();
    //    Task<VendorMasterBanking> GetVendorMasterBankingById(int id);
    //    Task<IEnumerable<VendorMasterBanking>> GetAllActiveVendorMasterBanking();
    //    Task<int?> CreateVendorMasterBanking(VendorMasterBanking vendorMasterBanking);
    //    Task<string> UpdateVendorMasterBanking(VendorMasterBanking vendorMasterBanking);
    //    Task<string> DeleteVendorMasterBanking(VendorMasterBanking vendorMasterBanking);

    //}

    //public interface IVendorRepository : IRepositoryBase<Address>
    //{
    //    Task<IEnumerable<Address>> GetAllDeliveryTerms();
    //    Task<Address> GetDeliveryTermById(int id);
    //    Task<IEnumerable<Address>> GetAllActiveDeliveryTerms();
    //    Task<int?> CreateDeliveryTerm(Address address);
    //    Task<string> UpdateDeliveryTerm(Address address);
    //    Task<string> DeleteDeliveryTerm(Address address);

    //}



}
