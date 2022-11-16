using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IWarehouseRepository : IRepositoryBase<Warehouse>
    {
        Task<IEnumerable<Warehouse>> GetAllWarehouse();
        Task<Warehouse> GetWarehouseById(int id);
        Task<IEnumerable<Warehouse>> GetAllActiveWarehouse();
        Task<int?> CreateWarehouse(Warehouse warehouse);
        Task<string> UpdateWarehouse(Warehouse warehouse);
        Task<string> DeleteWarehouse(Warehouse warehouse);
    }
}
