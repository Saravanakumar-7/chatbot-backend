
using Entities.Helper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<PagedList<FgOqc>> GetAllFgOqcs([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var getAllFgoqcDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.ProjectNumber.Contains(searchParammes.SearchValue) ||
                 inv.FGItemNumber.Contains(searchParammes.SearchValue) || inv.ShopOrderNumber.Contains(searchParammes.SearchValue))));

            return PagedList<FgOqc>.ToPagedList(getAllFgoqcDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

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
