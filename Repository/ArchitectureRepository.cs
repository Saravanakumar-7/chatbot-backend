using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class ArchitectureRepository : RepositoryBase<Architectures>, IArchitectureRepository
    {
        public ArchitectureRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateArchitecture(Architectures architecture)
        {
            architecture.CreatedBy = "Admin";
            architecture.CreatedOn = DateTime.Now;
            architecture.Unit = "Bangalore";
            var result = await Create(architecture); return result.Id;
        }
        public async Task<string> DeleteArchitecture(Architectures architecture)
        {
            Delete(architecture);
            string result = $"architecture details of {architecture.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<Architectures>> GetAllActiveArchitectures()
        {
            var allActiveArchitects = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return allActiveArchitects;
        }
        public async Task<IEnumerable<Architectures>> GetAllArchitectures()
        {
            var ArchitectsDetails = await FindAll().ToListAsync(); return ArchitectsDetails;
        }
        public async Task<Architectures> GetArchitectureById(int id)
        {
            var architectById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); 
            return architectById;
        }
        public async Task<string> UpdateArchitecture(Architectures architecture)
        {
            architecture.LastModifiedBy = "Admin";
            architecture.LastModifiedOn = DateTime.Now;
            Update(architecture);
            string result = $"architecture details of {architecture.Id} is updated successfully!";
            return result;
        }
    }
}