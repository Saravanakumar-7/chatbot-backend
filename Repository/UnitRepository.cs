using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UnitRepository : RepositoryBase<Unit>, IUnitRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public UnitRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateUnit(Unit unit)
        {
            unit.CreatedBy = _createdBy;
            unit.CreatedOn = DateTime.Now;
            var result = await Create(unit);

            return result.Id;
        }

        public async Task<string> DeleteUnit(Unit unit)
        {
            Delete(unit);
            string result = $"unit details of {unit.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Unit>> GetAllActiveUnits(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var getAllActiveUnits = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UnitName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<Unit>.ToPagedList(getAllActiveUnits, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<Unit>> GetAllUnits(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var getAllUnits = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UnitName.Contains(searchParams.SearchValue) ||
                    inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Unit>.ToPagedList(getAllUnits, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<Unit> GetUnitById(int id)
        {
            var unitByid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return unitByid;
        }

        public async Task<string> UpdateUnit(Unit unit)
        {
            unit.LastModifiedBy = _createdBy;
            unit.LastModifiedOn = DateTime.Now;
            Update(unit);
            string result = $"Unit details of {unit.Id} is updated successfully!";
            return result;
        }
    }
}
