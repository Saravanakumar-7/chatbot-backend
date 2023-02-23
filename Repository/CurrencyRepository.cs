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
            currency.Unit = "Bangalore";
            var result = await Create(currency);
            
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
            var AllActivecurrencies = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivecurrencies;
        }

        public async Task<IEnumerable<Currency>> GetAllCurrency()
        {
            var Getallcurrencies = await FindAll().ToListAsync();

            return Getallcurrencies;
        }

        public async Task<Currency> GetCurrencyById(int id)
        {
            var CurrencybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CurrencybyId;
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
