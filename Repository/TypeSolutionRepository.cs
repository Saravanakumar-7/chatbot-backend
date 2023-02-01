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
    public  class TypeSolutionRepository : RepositoryBase<TypeSolution>, ITypeSolutionRepository
    {
        public TypeSolutionRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateTypeSolution(TypeSolution typeSolution)
        {
            typeSolution.CreatedBy = "Admin";
            typeSolution.CreatedOn = DateTime.Now;
            typeSolution.Unit = "Bangalore";
            var result = await Create(typeSolution);

            return result.Id;
        }

        public async Task<string> DeleteTypeSolution(TypeSolution typeSolution)
        {
            Delete(typeSolution);
            string result = $"typeSolution details of {typeSolution.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<TypeSolution>> GetAllTypeSolutions()
        {
            var getAllTypeSolutions = await FindAll().OrderByDescending(x => x.Id).ToListAsync();

            return getAllTypeSolutions;
        }

        public async Task<TypeSolution> GetTypeSolutionById(int id)
        {
            var typeSolutionByid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return typeSolutionByid;
        }

        public async Task<string> UpdateTypeSolution(TypeSolution typeSolution)
        {
            typeSolution.LastModifiedBy = "Admin";
            typeSolution.LastModifiedOn = DateTime.Now;
            Update(typeSolution);
            string result = $"typeSolution details of {typeSolution.Id} is updated successfully!";
            return result;
        }
    }
}
