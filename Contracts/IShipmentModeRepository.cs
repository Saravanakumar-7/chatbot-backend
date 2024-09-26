using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IShipmentModeRepository
    {
        Task<IEnumerable<ShipmentMode>> GetAllShipmentModes(SearchParames searchParams);
        Task<ShipmentMode> GetShipmentModeById(int id);
        Task<IEnumerable<ShipmentMode>> GetAllActiveShipmentModes(SearchParames searchParams);
        Task<int?> CreateShipmentMode(ShipmentMode shipmentMode);
        Task<string> UpdateShipmentMode(ShipmentMode shipmentMode);
        Task<string> DeleteShipmentMode(ShipmentMode shipmentMode);
    }
}
