using Contracts;
using Entities;
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
            var result = await Create(vendorMaster);
            return result.Id;
        }

        public Task<string> DeleteVendor(VendorMaster vendorMaster)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<VendorMaster>> GetAllActiveVendors()
        {
            var vendorDetails = await FindAll().ToListAsync();

            return vendorDetails;
        }

        public async Task<IEnumerable<VendorMaster>> GetAllVendors()
        {
            var vendorDetails = await TipsMasterDbContext.VendorMasters
                                .Include(x=> x.Banking)
                                .Include(x=> x.Addresses)
                                .Include(m=> m.Contacts)
                                .ToListAsync();

            return vendorDetails;
        }

        public async Task<VendorMaster> GetVendorById(int id)
        {
            var vendorDetails = await TipsMasterDbContext.VendorMasters.Where(x=> x.Id == id)
                                .Include(x => x.Banking)
                                .Include(x => x.Addresses)
                                .Include(m => m.Contacts)
                                .FirstOrDefaultAsync();

            return vendorDetails;
        }

        public Task<string> UpdateVendor(VendorMaster vendorMaster)
        {
            throw new NotImplementedException();
        }
    }
}
