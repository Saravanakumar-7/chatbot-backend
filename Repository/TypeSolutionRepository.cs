using Contracts;
using Entities;
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
    public  class TypeSolutionRepository : RepositoryBase<TypeSolution>, ITypeSolutionRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public TypeSolutionRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateTypeSolution(TypeSolution typeSolution)
        {
            typeSolution.CreatedBy = _createdBy;
            typeSolution.CreatedOn = DateTime.Now;
            typeSolution.Unit = _unitname;
            var result = await Create(typeSolution);

            return result.Id;
        }

        public async Task<string> DeleteTypeSolution(TypeSolution typeSolution)
        {
            Delete(typeSolution);
            string result = $"typeSolution details of {typeSolution.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<TypeSolution>> GetAllTypeSolutions([FromQuery] SearchParames searchParams)
        {
            var typeSolutionDetails = FindAll()
                                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.TypeSolutionName.Contains(searchParams.SearchValue) ||
                                 inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return typeSolutionDetails;
        }

        public async Task<TypeSolution> GetTypeSolutionById(int id)
        {
            var typeSolutionByid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return typeSolutionByid;
        }

        public async Task<string> UpdateTypeSolution(TypeSolution typeSolution)
        {
            typeSolution.LastModifiedBy = _createdBy;
            typeSolution.LastModifiedOn = DateTime.Now;
            Update(typeSolution);
            string result = $"typeSolution details of {typeSolution.Id} is updated successfully!";
            return result;
        }
    }
}
