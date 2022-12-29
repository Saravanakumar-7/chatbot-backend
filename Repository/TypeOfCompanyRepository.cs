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
    public class TypeOfCompanyRepository : RepositoryBase<TypeOfCompany>, ITypeOfCompanyRepository

    {
        public TypeOfCompanyRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async  Task<int?> CreateTypeOfCompany(TypeOfCompany typeOfCompany)
        {
            typeOfCompany.CreatedBy = "Admin";
            typeOfCompany.CreatedOn = DateTime.Now;
            typeOfCompany.Unit = "Bangalore";
            var result = await Create(typeOfCompany);
            
            return result.Id;
        }

        public async Task<string> DeleteTypeOfCompany(TypeOfCompany typeOfCompany)
        {
            Delete(typeOfCompany);
            string result = $"AuditFrequency details of {typeOfCompany.Id} is deleted successfully!";
            return result;
        }      

        public async Task<IEnumerable<TypeOfCompany>> GetAllActiveTypeofCompanies()
        {
            var AllActiveTypeOfCompanyList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveTypeOfCompanyList;
        }

        public async Task<IEnumerable<TypeOfCompany>> GetAllTypeOfCompanies()
        {

            var GetallTypeOfCompany = await FindAll().ToListAsync();

            return GetallTypeOfCompany;
        }

        public async Task<TypeOfCompany> GetTypeOfCompanyById(int id)
        {

            var TypeOfCompanybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return TypeOfCompanybyId;
        }

        public async Task<string> UpdateTypeOfCompany(TypeOfCompany typeOfCompany)
        {

            typeOfCompany.LastModifiedBy = "Admin";
            typeOfCompany.LastModifiedOn = DateTime.Now;
            Update(typeOfCompany);
            string result = $"TypeOfCompany details of {typeOfCompany.Id} is updated successfully!";
            return result;
        }
    }
}
