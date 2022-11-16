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
    public class UOCRepository : RepositoryBase<UOC>, IUOCRepository
    {
        public UOCRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateUOC(UOC uoc)
        {
            uoc.CreatedBy = "Admin";
            uoc.CreatedOn = DateTime.Now;
            var result = await Create(uoc);
            return result.Id;
        }

        public async Task<string> DeleteUOC(UOC uoc)
        {
            Delete(uoc);
            string result = $"UOC details of {uoc.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<UOC>> GetAllActiveUOC()
        {
            var uocList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return uocList;
        }

        public async Task<IEnumerable<UOC>> GetAllUOC()
        {
            var uocList = await FindAll().ToListAsync();
            return uocList;
        }

        public async Task<UOC> GetUOCById(int id)
        {
            var uocList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return uocList;
        }

        public async Task<string> UpdateUOC(UOC uoc)
        {
            uoc.LastModifiedBy = "Admin";
            uoc.LastModifiedOn = DateTime.Now;
            Update(uoc);
            string result = $"UOC details of {uoc.Id} is updated successfully!";
            return result;
        }
    }
}
