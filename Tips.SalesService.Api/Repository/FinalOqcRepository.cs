
using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Repository
{
    public class FinalOqcRepository : RepositoryBase<FinalOqc>, IFinalOqcRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public FinalOqcRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }

        public async Task<int?> CreateFinalOqc(FinalOqc finalOqc)
        {
            finalOqc.CreatedBy = "Admin";
            finalOqc.CreatedOn = DateTime.Now;
            finalOqc.Unit = "Bangalore";
            var result = await Create(finalOqc);           
            return result.Id;
        }



        public async Task<string> DeleteFinalOqc(FinalOqc finalOqc)
        {
            Delete(finalOqc);
            string result = $"FinalOqc details of {finalOqc.Id} is deleted successfully!";
            return result;
        }


        public async Task<PagedList<FinalOqc>> GetAllFinalOqc(PagingParameter pagingParameter)
        {
            var getAllFinalOqc = PagedList<FinalOqc>.ToPagedList(FindAll()
           .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return getAllFinalOqc;

        }




        public async Task<FinalOqc> GetFinalOqcById(int id)
        {
            var getFinalOqcById = await _tipsSalesServiceDbContext.FinalOqcs.Where(x => x.Id == id).FirstOrDefaultAsync();
            return getFinalOqcById;
        }



        public async Task<string> UpdateFinalOqc(FinalOqc finalOqc)
        {
            finalOqc.LastModifiedBy = "Admin";
            finalOqc.LastModifiedOn = DateTime.Now;
            Update(finalOqc);
            string result = $"fgOqc of Detail {finalOqc.Id} is updated successfully!";
            return result;
        }

    }
}
