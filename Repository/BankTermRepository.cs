using Contracts;
using Entities;
using Entities.Migrations;
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
            var result = await Create(bank);
            bank.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteBank(Bank bank)
        {
            Delete(bank);
            string result = $"Bank details of {bank.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Bank>> GetAllActiveBank()
        {
            var banks = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return banks;
        }

        public async Task<IEnumerable<Bank>> GetAllBank()
        {
            var banks = await FindAll().ToListAsync();

            return banks;
        }

        public async Task<Bank> GetBankById(int id)
        {
            var bankById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return bankById;
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
