using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface IItemMasterRoutingRepository
    {
        
        Task<List<ItemMasterRoutingListDto>> GetItemsRoutingDetailsForLpCosting(List<string> itemNumberList); 
        
    }
}
