
using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Repository
{
    public class SaOqcRepository : RepositoryBase<SaOqc>, ISaOqcRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public SaOqcRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }

        public async Task<int?> CreateSaOqc(SaOqc saOqc)
        {
            saOqc.CreatedBy = "Admin";
            saOqc.CreatedOn = DateTime.Now;
            var result = await Create(saOqc);
            saOqc.Unit = "Bangalore";
            return result.Id;
        }



        public async Task<string> DeleteSaOqc(SaOqc saOqc)
        {
            Delete(saOqc);
            string result = $"saOqc details of {saOqc.Id} is deleted successfully!";
            return result;
        }


        public async Task<PagedList<SaOqc>> GetAllSaOqc(PagingParameter pagingParameter)
        {
            var saqoc = PagedList<SaOqc>.ToPagedList(FindAll()
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return saqoc;

        }




        public async Task<SaOqc> GetSaOqcById(int id)
        {
            var saQocs = await _tipsSalesServiceDbContext.SaOqcs.Where(x => x.Id == id).FirstOrDefaultAsync();
            return saQocs;
        }



        public async Task<string> UpdateSaOqc(SaOqc saOqc)
        {
            saOqc.CreatedBy = "Admin";
            saOqc.CreatedOn = DateTime.Now;
            Update(saOqc);
            string result = $"fgOqc of Detail {saOqc.Id} is updated successfully!";
            return result;
        }

    }
}
