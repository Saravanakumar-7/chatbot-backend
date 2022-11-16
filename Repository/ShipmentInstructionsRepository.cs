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
    public class ShipmentInstructionsRepository : RepositoryBase<ShipmentInstructions>, IShipmentInstructionsRepository
    {
        public ShipmentInstructionsRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateShipmentInstructions(ShipmentInstructions shipmentInstructions)
        {
            shipmentInstructions.CreatedBy = "Admin";
            shipmentInstructions.CreatedOn = DateTime.Now;
            var result = await Create(shipmentInstructions);
            return result.Id;
        }

        public async Task<string> DeleteShipmentInstructions(ShipmentInstructions shipmentInstructions)
        {
            Delete(shipmentInstructions);
            string result = $"ShipmentInstructions details of {shipmentInstructions.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ShipmentInstructions>> GetAllActiveShipmentInstructions()
        {
            var shipmentInstructionsList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return shipmentInstructionsList;
        }

        public async Task<IEnumerable<ShipmentInstructions>> GetAllShipmentInstructions()
        {
            var shipmentInstructionsList = await FindAll().ToListAsync();
            return shipmentInstructionsList;
        }

        public async Task<ShipmentInstructions> GetShipmentInstructionsById(int id)
        {
            var shipmentInstructionsList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return shipmentInstructionsList;
        }

        public async Task<string> UpdateShipmentInstructions(ShipmentInstructions shipmentInstructions)
        {
            shipmentInstructions.LastModifiedBy = "Admin";
            shipmentInstructions.LastModifiedOn = DateTime.Now;
            Update(shipmentInstructions);
            string result = $"ShipmentInstructions details of {shipmentInstructions.Id} is updated successfully!";
            return result;
        }
    }
}
