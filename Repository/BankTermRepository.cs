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
    public class BankTermRepository : RepositoryBase<Bank>, IBankRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BankTermRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateBank(Bank bank)
        {
            bank.CreatedBy = _createdBy;
            bank.CreatedOn = DateTime.Now;
            bank.Unit = _unitname;
            var result = await Create(bank);
            return result.Id;
        }

        public async Task<string> DeleteBank(Bank bank)
        {
            Delete(bank);
            string result = $"Bank details of {bank.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Bank>> GetAllActiveBank([FromQuery] SearchParames searchParams)
        {
            var bankDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BankName.Contains(searchParams.SearchValue) ||
           inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return bankDetails;
        }

        public async Task<IEnumerable<Bank>> GetAllBank([FromQuery] SearchParames searchParams)
        {
            var bankDetails = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BankName.Contains(searchParams.SearchValue) ||
           inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return bankDetails;
        }


        public async Task<Bank> GetBankById(int id)
        {
            var BankById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return BankById;
        }

        public async Task<string> UpdateBank(Bank bank)
        {
            bank.LastModifiedBy = _createdBy;
            bank.LastModifiedOn = DateTime.Now;
            Update(bank);
            string result = $"Bank of Detail {bank.Id} is updated successfully!";
            return result;
        }
    }
}
