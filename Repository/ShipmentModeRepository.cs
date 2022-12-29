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
    public class ShipmentModeRepository : RepositoryBase<ShipmentMode>, IShipmentModeRepository
    {
        public ShipmentModeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateShipmentMode(ShipmentMode shipmentMode)
        {
            shipmentMode.CreatedBy = "Admin";
            shipmentMode.CreatedOn = DateTime.Now;
            shipmentMode.Unit = "Bangalore";
            var result = await Create(shipmentMode);

            return result.Id;
        }

        public async Task<string> DeleteShipmentMode(ShipmentMode shipmentMode)
        {
            Delete(shipmentMode);
            string result = $"ShipmentMode details of {shipmentMode.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ShipmentMode>> GetAllActiveShipmentModes()
        {

            var AllActiveShipmentModes = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveShipmentModes;
        }

        public async Task<IEnumerable<ShipmentMode>> GetAllShipmentModes()
        {
            var GetallShipmentModes = await FindAll().ToListAsync();

            return GetallShipmentModes;
        }

        public async Task<ShipmentMode> GetShipmentModeById(int id)
        {
            var ShipmentModebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return ShipmentModebyId;
        }

        public async Task<string> UpdateShipmentMode(ShipmentMode shipmentMode)
        {
            shipmentMode.LastModifiedBy = "Admin";
            shipmentMode.LastModifiedOn = DateTime.Now;
            Update(shipmentMode);
            string result = $"shipmentMode details of {shipmentMode.Id} is updated successfully!";
            return result;
        }
    }
}
