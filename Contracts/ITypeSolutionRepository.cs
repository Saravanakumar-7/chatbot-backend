using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public  interface ITypeSolutionRepository : IRepositoryBase<TypeSolution>
    {
        Task<IEnumerable<TypeSolution>> GetAllTypeSolutions(SearchParames searchParams);
        Task<IEnumerable<TypeSolution>> GetAllActiveTypeSolutions(SearchParames searchParams);
        Task<TypeSolution> GetTypeSolutionById(int id);
        Task<int?> CreateTypeSolution(TypeSolution typeSolution);
        Task<string> UpdateTypeSolution(TypeSolution typeSolution);
        Task<string> DeleteTypeSolution(TypeSolution typeSolution);
    }
}
