using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class VendorRepository : RepositoryBase<VendorMaster>, IVendorRepository
    {
        public VendorRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateVendor(VendorMaster vendorMaster)
        {
                vendorMaster.CreatedBy = "Admin";
                vendorMaster.CreatedOn = DateTime.Now;
                vendorMaster.LastModifiedBy = "Admin";
                vendorMaster.LastModifiedOn = DateTime.Now; 
            
                var result = await Create(vendorMaster);
            vendorMaster.Unit = "Bangalore";
            return result.Id; 

        }
         

        //public async Task<string> DeleteVendor(VendorMasterDto vendorMasterDto)
        //{
        //    //Delete(vendorMasterDto.Address);
        //    //string result = $"Delete Vendor details of {vendorMasterDto.Id} is deleted successfully!";
        //    //return result;
        //    throw new NotImplementedException();

        //}

        public async Task<IEnumerable<VendorMaster>> GetAllActiveVendors()
        {
            var vendorDetails = await FindAll().ToListAsync();
            return vendorDetails;
            //throw new NotImplementedException();

        }

        public async Task<PagedList<VendorMaster>> GetAllVendors(PagingParameter pagingParameter)
        {

            var vendorDetails = PagedList<VendorMaster>.ToPagedList(FindAll()
                                .Include(t => t.VendorBankings)
                                .Include(x => x.Addresses)
                                .Include(m => m.Contacts)
                                .Include(v => v.HeadCountings)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            //var vendorDetails = await TipsMasterDbContext.VendorMasters
            //                    .Include(t=> t.VendorBankings)
            //                    .Include(x=> x.Addresses)
            //                    .Include(m=> m.Contacts)
            //                    .ToListAsync();

            return vendorDetails;
            //throw new NotImplementedException();


        }

        public async Task<VendorMaster> GetVendorById(int id)
        {
          var vendorDetails = await TipsMasterDbContext.VendorMasters.Where(x=> x.Id == id)
                              .Include(x => x.VendorBankings)
                              .Include(x => x.Addresses)
                              .Include(m => m.Contacts)
                              .Include(v => v.HeadCountings)
                              .FirstOrDefaultAsync();

          return vendorDetails;
          //  throw new NotImplementedException();
        }
         

        public async Task<string> UpdateVendor(VendorMaster DataUpdate)             
        {
            DataUpdate.LastModifiedBy = "Admin";
            DataUpdate.LastModifiedOn = DateTime.Now; 
            Update(DataUpdate);
            string result = $"Vendor of Detail {DataUpdate.Id} is updated successfully!";
            return result; 
        }

        public async Task<string> DeleteVendor(VendorMaster vendormaster)
        {
            Delete(vendormaster);
            string result = $"Vendor details of {vendormaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<VendorIdNameListDto>> GetAllActiveVendorNameList()
        {
            IEnumerable<VendorIdNameListDto> vendorDetails = await TipsMasterDbContext.VendorMasters
                                .Select(x => new VendorIdNameListDto()
                                {
                                    Id = x.Id,
                                    VednorAliasName = x.VendorAliasName,
                                    VendorName = x.VendorName
                                })
                              .ToListAsync();

            return vendorDetails;
        }
         
    }
}
