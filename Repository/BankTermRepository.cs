using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BankTermRepository : RepositoryBase<Bank>, IBankRepository
    {
        public BankTermRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateBank(Bank bank)
        {
            bank.CreatedBy = "Admin";
            bank.CreatedOn = DateTime.Now;
            bank.Unit = "Bangalore";
            var result = await Create(bank);
            return result.Id;
        }

        public async Task<string> DeleteBank(Bank bank)
        {
            Delete(bank);
            string result = $"Bank details of {bank.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Bank>> GetAllActiveBank([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var getAllAActiveBanks = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BankName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<Bank>.ToPagedList(getAllAActiveBanks, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<Bank>> GetAllBank([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var getAllBanksDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BankName.Contains(searchParams.SearchValue) ||
                inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Bank>.ToPagedList(getAllBanksDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<Bank> GetBankById(int id)
        {
            var BankById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return BankById;
        }

        public async Task<string> UpdateBank(Bank bank)
        {
            bank.LastModifiedBy = "Admin";
            bank.LastModifiedOn = DateTime.Now;
            Update(bank);
            string result = $"Bank of Detail {bank.Id} is updated successfully!";
            return result;
        }
    }
}
