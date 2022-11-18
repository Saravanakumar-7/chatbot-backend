using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class UOMRepository : RepositoryBase<UOM>, IUOMRepository
    {
        public UOMRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateUOM(UOM uom)
        {
            uom.CreatedBy = "Admin";
            uom.CreatedOn = DateTime.Now;
            var result = await Create(uom);
            uom.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteUOM(UOM uom)
        {
            Delete(uom);
            string result = $"UOM details of {uom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<UOM>> GetAllActiveUOM()
        {
            var uomList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return uomList;
        }
        public async Task<IEnumerable<UOM>> GetAllUOM()
        {
            var uomList = await FindAll().ToListAsync();
            return uomList;
        }

        public async Task<UOM> GetUOMById(int id)
        {
            var uomList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return uomList;
        }

        public async Task<string> UpdateUOM(UOM uom)
        {
            uom.LastModifiedBy = "Admin";
            uom.LastModifiedOn = DateTime.Now;
            Update(uom);
            string result = $"UOM details of {uom.Id} is updated successfully!";
            return result;
        }
    }
}
