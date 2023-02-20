
using Entities.Helper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{

    public class FgOqcRepository : RepositoryBase<FgOqc>, IFgOqcRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public FgOqcRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }

        public async Task<int?> CreateFgOqc(FgOqc fgOqc)
        {
            fgOqc.CreatedBy = "Admin";
            fgOqc.CreatedOn = DateTime.Now;
            fgOqc.Unit = "Bangalore";
            var result = await Create(fgOqc);            
            return result.Id;
        }

        public async Task<string> DeleteFgOqc(FgOqc fgOqc)
        {
            Delete(fgOqc);
            string result = $"fgOqc details of {fgOqc.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<FgOqc>> GetAllFgOqcs(PagingParameter pagingParameter)
        {
            var getAllFgOqcs = PagedList<FgOqc>.ToPagedList(FindAll()
           .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return getAllFgOqcs;

        }


        public async Task<FgOqc> GetFgOqcById(int id)
        {
            var getFgOqcById = await _tipsSalesServiceDbContext.FgOqcs.Where(x => x.Id == id).FirstOrDefaultAsync();
            return getFgOqcById;
        }

        public async Task<string> UpdateFgOqc(FgOqc fgOqc)
        {
            fgOqc.LastModifiedBy = "Admin";
            fgOqc.LastModifiedOn = DateTime.Now;
            Update(fgOqc);
            string result = $"fgOqc of Detail {fgOqc.Id} is updated successfully!";
            return result;
        }
    }
}
