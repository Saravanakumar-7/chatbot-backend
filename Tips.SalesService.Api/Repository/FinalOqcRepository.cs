
using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tips.SalesService.Api.Repository
{
    public class FinalOqcRepository : RepositoryBase<FinalOqc>, IFinalOqcRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public FinalOqcRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateFinalOqc(FinalOqc finalOqc)
        {
            finalOqc.CreatedBy = _createdBy;
            finalOqc.CreatedOn = DateTime.Now;
            finalOqc.Unit = _unitname;
            var result = await Create(finalOqc);           
            return result.Id;
        }



        public async Task<string> DeleteFinalOqc(FinalOqc finalOqc)
        {
            Delete(finalOqc);
            string result = $"FinalOqc details of {finalOqc.Id} is deleted successfully!";
            return result;
        }


        public async Task<PagedList<FinalOqc>> GetAllFinalOqc([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var getAllFinalFgoqcDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.ProjectNumber.Contains(searchParammes.SearchValue) ||
                 inv.FGItemNumber.Contains(searchParammes.SearchValue) || inv.ShopOrderNumber.Contains(searchParammes.SearchValue))));

            return PagedList<FinalOqc>.ToPagedList(getAllFinalFgoqcDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }




        public async Task<FinalOqc> GetFinalOqcById(int id)
        {
            var getFinalOqcById = await _tipsSalesServiceDbContext.FinalOqcs.Where(x => x.Id == id).FirstOrDefaultAsync();
            return getFinalOqcById;
        }



        public async Task<string> UpdateFinalOqc(FinalOqc finalOqc)
        {
            finalOqc.LastModifiedBy = _createdBy;
            finalOqc.LastModifiedOn = DateTime.Now;
            Update(finalOqc);
            string result = $"fgOqc of Detail {finalOqc.Id} is updated successfully!";
            return result;
        }

    }
}
