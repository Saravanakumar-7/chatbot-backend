using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    internal class SalutationsRepository : RepositoryBase<Salutations>, ISalutationsRepository
    {
        public SalutationsRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateSalutations(Salutations salutations)
        {
            salutations.CreatedBy = "Admin";
            salutations.CreatedOn = DateTime.Now;
            salutations.Unit = "Bangalore";
            var result = await Create(salutations);
            
            return result.Id;
        }

        public async Task<string> DeleteSalutations(Salutations salutations)
        {

            Delete(salutations);
            string result = $"Salutations details of {salutations.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Salutations>> GetAllActiveSalutations()
        {
            var AllActivesalutations= await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivesalutations;
        }

        public async Task<IEnumerable<Salutations>> GetAllSalutations()
        {

            var GetallSalutations= await FindAll().ToListAsync();

            return GetallSalutations;
        }

        public async Task<Salutations> GetSalutationsById(int id)
        {
            var SalutationsbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return SalutationsbyId;
        }

        public async Task<string> UpdateSalutations(Salutations salutations)
        {
            salutations.LastModifiedBy = "Admin";
            salutations.LastModifiedOn = DateTime.Now;
            Update(salutations);
            string result = $"Salutations details of {salutations.Id} is updated successfully!";
            return result;
        }
    }
}
