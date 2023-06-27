using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CurrencyRepository : RepositoryBase<Currency>, ICurrencyRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CurrencyRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateCurrency(Currency currency)
        {
            currency.CreatedBy = _createdBy;
            currency.CreatedOn = DateTime.Now;
            currency.Unit = _unitname;
            var result = await Create(currency);
            
            return result.Id;
        }

        public async Task<string> DeleteCurrency(Currency currency)
        {
            Delete(currency);
            string result = $"Currency details of {currency.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Currency>> GetAllActiveCurrency([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var currencyDetails = FindAll()
                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CurrencyName.Contains(searchParams.SearchValue) ||
                      inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<Currency>.ToPagedList(currencyDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<Currency>> GetAllCurrency([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var currencyDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CurrencyName.Contains(searchParams.SearchValue) ||
             inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Currency>.ToPagedList(currencyDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<Currency> GetCurrencyById(int id)
        {
            var CurrencybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CurrencybyId;
        }

        public async Task<string> UpdateCurrency(Currency currency)
        {
            currency.LastModifiedBy = _createdBy;
            currency.LastModifiedOn = DateTime.Now;
            Update(currency);
            string result = $"Currency of Detail {currency.Id} is updated successfully!";
            return result;
        }
    }
}
