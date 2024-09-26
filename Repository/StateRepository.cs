using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class StateRepository : RepositoryBase<State>, IStateRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public StateRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateState(State state)
        {
            state.CreatedBy = _createdBy;
            state.CreatedOn = DateTime.Now;
            state.Unit = _unitname;
            var result = await Create(state); return result.Id;
        }
        public async Task<string> DeleteState(State state)
        {
            Delete(state);
            string result = $"state details of {state.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<State>> GetAllActiveStates()
        {
            var AllActivestates = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivestates;
        }
        public async Task<IEnumerable<State>> GetAllStates()
        {
            var GetallStates = await FindAll().ToListAsync(); return GetallStates;
        }
        public async Task<State> GetStateById(int id)
        {
            var statebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return statebyId;
        }
        public async Task<string> UpdateState(State state)
        {
            state.LastModifiedBy = _createdBy;
            state.LastModifiedOn = DateTime.Now;
            Update(state);
            string result = $"state details of {state.Id} is updated successfully!";
            return result;
        }
    }
}