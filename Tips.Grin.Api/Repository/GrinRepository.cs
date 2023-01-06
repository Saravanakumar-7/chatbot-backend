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
using Entities.DTOs;

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

            grins.Unit = "Bangalore";
            

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
            var allActiveGrinDetails = await FindAll().ToListAsync();
            return allActiveGrinDetails;
        }

        public async Task<IEnumerable<GrinNumberListDto>> GetAllActiveGrinNoList()
        {
            IEnumerable<GrinNumberListDto> allActiveGrinNoList= await _tipsGrinDbContext.Grins
                                .Select(x => new GrinNumberListDto()
                                {                                  

                                    Id= x.Id,
                                    GrinNumber=x.GrinNumber
                                })
                              .ToListAsync();

            return allActiveGrinNoList;
        }

        public async Task<PagedList<Grins>> GetAllGrin(PagingParameter pagingParameter)
        {
            var getAllGrinDetails= PagedList<Grins>.ToPagedList (FindAll()
                                .Include(t => t.GrinParts)
                                .ThenInclude(t => t.ProjectNumbers)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllGrinDetails;
        }

        public async Task<Grins> GetGrinById(int id)
        {
            var grinDetailsbyId = await _tipsGrinDbContext.Grins.Where(x => x.Id == id)
                               .Include(x => x.GrinParts)
                               .ThenInclude(t => t.ProjectNumbers)
                               .FirstOrDefaultAsync();

            return grinDetailsbyId;
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
