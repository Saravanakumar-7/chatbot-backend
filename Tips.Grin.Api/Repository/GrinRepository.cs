using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Entities;
using Tips.Grin.Api.Entities.DTOs;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Tips.Grin.Api.Repository
{
    public class GrinRepository : RepositoryBase<Grins>, IGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        public GrinRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
        } 
        public async Task<int?> CreateGrin(Grins grins)
        {
            grins.CreatedBy = "Admin";
            grins.CreatedOn = DateTime.Now;
            grins.LastModifiedBy = "Admin";
            grins.LastModifiedOn = DateTime.Now;

            var result = await Create(grins);
            return result.Id;
        }
        
        public async Task<string> DeleteGrin(Grins grins)
        {
            Delete(grins);
            string result = $"Grin details of {grins.Id} is deleted successfully!";
            return result;
        } 

        public async Task<IEnumerable<Grins>> GetAllActiveGrin()
        {
            var GrinDetails = await FindAll().ToListAsync();
            return GrinDetails;
        }

        public async Task<PagedList<Grins>> GetAllGrin(PagingParameter pagingParameter)
        {
            var grinDetails = PagedList<Grins>.ToPagedList(FindAll()
                                .Include(t => t.GrinParts)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return grinDetails;
        }

        public async Task<Grins> GetGrinById(int id)
        {
            var grinDetails = await _tipsGrinDbContext.Grins.Where(x => x.Id == id)
                               .Include(x => x.GrinParts)
                               .FirstOrDefaultAsync();

            return grinDetails;
        }
         

        public async Task<string> UpdateGrin(Grins grins)
        {
            grins.LastModifiedBy = "Admin";
            grins.LastModifiedOn = DateTime.Now;
            Update(grins);
            string result = $"Grin Detail {grins.Id} is updated successfully!";
            return result;
        }

       

    }
}
