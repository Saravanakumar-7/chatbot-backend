using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class BHKRepository : RepositoryBase<BHK>, IBHKRepository
    {
        public BHKRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateBHK(BHK bHK)
        {
            bHK.CreatedBy = "Admin";
            bHK.CreatedOn = DateTime.Now;
            bHK.Unit = "Bangalore";
            var result = await Create(bHK); return result.Id;
        }
        public async Task<string> DeleteBHK(BHK bHK)
        {
            Delete(bHK);
            string result = $"bHK details of {bHK.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<BHK>> GetAllActiveBHK()
        {
            var AllActiveBHK = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveBHK;
        }
        public async Task<IEnumerable<BHK>> GetAllBHK()
        {
            var GetallBhk = await FindAll().ToListAsync(); return GetallBhk;
        }
        public async Task<BHK> GetBHKById(int id)
        {
            var BHKyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return BHKyId;
        }
        public async Task<string> UpdateBHK(BHK bHK)
        {
            bHK.LastModifiedBy = "Admin";
            bHK.LastModifiedOn = DateTime.Now;
            Update(bHK);
            string result = $"bHK details of {bHK.Id} is updated successfully!";
            return result;
        }
    }
}