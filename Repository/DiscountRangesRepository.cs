using Contracts;
using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Repository
{
    internal class DiscountRangesRepository : RepositoryBase<DiscountRanges>, IDiscountRangesRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public DiscountRangesRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateDiscountRanges(DiscountRanges discountRanges)
        {

            var result = await Create(discountRanges);

            return result.Id;
        }
        public async Task<DiscountRanges> GetDiscountRangesById(int id)
        {
            var discountRange = await FindByCondition(x => x.Id == id)
                .FirstOrDefaultAsync();
            return discountRange;
        }

        public async Task<DiscountRanges> GetDiscountRangesByAmount(decimal amount)
        {
            var DiscountRangesbyProcurementType = await FindByCondition(x =>
                x.FromAmount <= amount &&
                amount <= (x.ToAmount ?? decimal.MaxValue))
                .FirstOrDefaultAsync();

            return DiscountRangesbyProcurementType;
        }


        public async Task<PagedList<DiscountRanges>> GetAllDiscountRanges(PagingParameter pagingParameter)
        {
            var query = FindAll().OrderBy(x => x.Id);
            return PagedList<DiscountRanges>.ToPagedList(
              query,
              pagingParameter.PageNumber, pagingParameter.PageSize
            );
        }


        public async Task UpdateDiscountRanges(DiscountRanges discountRanges)
        {
            Update(discountRanges);
        }

    }
}
