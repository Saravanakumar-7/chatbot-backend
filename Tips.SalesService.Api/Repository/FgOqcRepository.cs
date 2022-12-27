
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
            var result = await Create(fgOqc);
            fgOqc.Unit = "Bangalore";
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
            var fgqoc = PagedList<FgOqc>.ToPagedList(FindAll()
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return fgqoc;

        }


        public async Task<FgOqc> GetFgOqcById(int id)
        {
            var fgQocs = await _tipsSalesServiceDbContext.FgOqcs.Where(x => x.Id == id).FirstOrDefaultAsync();
            return fgQocs;
        }

        public async Task<string> UpdateFgOqc(FgOqc fgOqc)
        {
            fgOqc.CreatedBy = "Admin";
            fgOqc.CreatedOn = DateTime.Now;
            Update(fgOqc);
            string result = $"fgOqc of Detail {fgOqc.Id} is updated successfully!";
            return result;
        }
    }
}
