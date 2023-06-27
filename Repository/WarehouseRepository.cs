using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class WarehouseRepository : RepositoryBase<Warehouse>, IWarehouseRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public WarehouseRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateWarehouse(Warehouse warehouse)
        {
            warehouse.CreatedBy = _createdBy;
            warehouse.CreatedOn = DateTime.Now;
            warehouse.Unit = _unitname;
            var result = await Create(warehouse);
            
            return result.Id;
        }

        public async Task<string> DeleteWarehouse(Warehouse warehouse)
        {
            Delete(warehouse);
            string result = $"Warehouse details of {warehouse.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Warehouse>> GetAllActiveWarehouse()
        {
            var AllActiveWarehouseList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return AllActiveWarehouseList;
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehouse()
        {
            var GetallWarehouses = await FindAll().ToListAsync();
            return GetallWarehouses;
        }

        public async Task<Warehouse> GetWarehouseById(int id)
        {
            var WarehousebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return WarehousebyId;
        }

        public async Task<string> UpdateWarehouse(Warehouse warehouse)
        {
            warehouse.LastModifiedBy = _createdBy;
            warehouse.LastModifiedOn = DateTime.Now;
            Update(warehouse);
            string result = $"Warehouse details of {warehouse.Id} is updated successfully!";
            return result;
        }
    }
}
