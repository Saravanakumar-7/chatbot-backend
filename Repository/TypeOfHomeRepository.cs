using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class TypeOfHomeRepository : RepositoryBase<TypeOfHome>, ITypeOfHomeRepository
    {
        public TypeOfHomeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateTypeOfHome(TypeOfHome typeOfHome)
        {
            typeOfHome.CreatedBy = "Admin";
            typeOfHome.CreatedOn = DateTime.Now;
            typeOfHome.Unit = "Bangalore";
            var result = await Create(typeOfHome); return result.Id;
        }
        public async Task<string> DeleteTypeOfHome(TypeOfHome typeOfHome)
        {
            Delete(typeOfHome);
            string result = $"typeOfHome details of {typeOfHome.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<TypeOfHome>> GetAllActiveTypeOfHome()
        {
            var AllActivetypeOfHome = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivetypeOfHome;
        }
        public async Task<IEnumerable<TypeOfHome>> GetAllTypeOfHome()
        {
            var GetalltypeOfHome = await FindAll().ToListAsync(); return GetalltypeOfHome;
        }
        public async Task<TypeOfHome> GetTypeOfHomeById(int id)
        {
            var typeOfHomebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return typeOfHomebyId;
        }
        public async Task<string> UpdateTypeOfHome(TypeOfHome typeOfHome)
        {
            typeOfHome.LastModifiedBy = "Admin";
            typeOfHome.LastModifiedOn = DateTime.Now;
            Update(typeOfHome);
            string result = $"typeOfHome details of {typeOfHome.Id} is updated successfully!";
            return result;
        }
    }
}