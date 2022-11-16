using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class WarehouseRepository : RepositoryBase<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateWarehouse(Warehouse warehouse)
        {
            warehouse.CreatedBy = "Admin";
            warehouse.CreatedOn = DateTime.Now;
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
            var warehouseList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return warehouseList;
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehouse()
        {
            var warehouseList = await FindAll().ToListAsync();
            return warehouseList;
        }

        public async Task<Warehouse> GetWarehouseById(int id)
        {
            var warehouseList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return warehouseList;
        }

        public async Task<string> UpdateWarehouse(Warehouse warehouse)
        {
            warehouse.LastModifiedBy = "Admin";
            warehouse.LastModifiedOn = DateTime.Now;
            Update(warehouse);
            string result = $"Warehouse details of {warehouse.Id} is updated successfully!";
            return result;
        }
    }
}
