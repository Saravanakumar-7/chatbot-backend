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
    public class VendorDepartmentRepository : RepositoryBase<VendorDepartment>, IVendorDepartmentRepository
    {
        public VendorDepartmentRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateVendorDepartment(VendorDepartment vendorDepartment)
        {
            vendorDepartment.CreatedBy = "Admin";
            vendorDepartment.CreatedOn = DateTime.Now;
            var result = await Create(vendorDepartment);
            return result.Id;
        }

        public async Task<string> DeleteVendorDepartment(VendorDepartment vendorDepartment)
        {
            Delete(vendorDepartment);
            string result = $"Vendor Department details of {vendorDepartment.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<VendorDepartment>> GetAllActiveVendorDepartment()
        {
            var vendorDepartments = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return vendorDepartments;
        }

        public async Task<IEnumerable<VendorDepartment>> GetAllVendorDepartment()
        {
            var vendorDepartments = await FindAll().ToListAsync();

            return vendorDepartments;
        }

        public async Task<VendorDepartment> GetVendorDepartmentById(int id)
        {
            var vendorDepartment = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return vendorDepartment;
        }

        public async Task<string> UpdateVendorDepartment(VendorDepartment vendorDepartment)
        {
            vendorDepartment.LastModifiedBy = "Admin";
            vendorDepartment.LastModifiedOn = DateTime.Now;
            Update(vendorDepartment);
            string result = $"Vendor Department of Detail {vendorDepartment.Id} is updated successfully!";
            return result;
        }
    }
}
