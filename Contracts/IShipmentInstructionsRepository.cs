using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IShipmentInstructionsRepository : IRepositoryBase<ShipmentInstructions>
    {
        Task<IEnumerable<ShipmentInstructions>> GetAllShipmentInstructions();
        Task<ShipmentInstructions> GetShipmentInstructionsById(int id);
        Task<IEnumerable<ShipmentInstructions>> GetAllActiveShipmentInstructions();
        Task<int?> CreateShipmentInstructions(ShipmentInstructions shipmentInstructions);
        Task<string> UpdateShipmentInstructions(ShipmentInstructions shipmentInstructions);
        Task<string> DeleteShipmentInstructions(ShipmentInstructions shipmentInstructions);
    }
    
}
