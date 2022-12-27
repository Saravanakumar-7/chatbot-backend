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
    public class CurrencyRepository : RepositoryBase<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateCurrency(Currency currency)
        {
            currency.CreatedBy = "Admin";
            currency.CreatedOn = DateTime.Now;
            var result = await Create(currency);
            currency.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteCurrency(Currency currency)
        {
            Delete(currency);
            string result = $"Currency details of {currency.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Currency>> GetAllActiveCurrency()
        {
            var currencies = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return currencies;
        }

        public async Task<IEnumerable<Currency>> GetAllCurrency()
        {
            var currencies = await FindAll().ToListAsync();

            return currencies;
        }

        public async Task<Currency> GetCurrencyById(int id)
        {
            var currency = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return currency;
        }

        public async Task<string> UpdateCurrency(Currency currency)
        {
            currency.LastModifiedBy = "Admin";
            currency.LastModifiedOn = DateTime.Now;
            Update(currency);
            string result = $"Currency of Detail {currency.Id} is updated successfully!";
            return result;
        }
    }
}
