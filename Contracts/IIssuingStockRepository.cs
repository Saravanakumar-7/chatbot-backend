using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IIssuingStockRepository : IRepositoryBase<IssuingStock>

    {
        Task<int?> CreateIssuingStock(IssuingStock issuingStock);
        Task<string> UpdateIssuingStock(IssuingStock issuingStock);
        Task<string> DeleteIssuingStock(IssuingStock issuingStock);
        Task<IssuingStock> GetIssuingStockById(int id);
        Task<IEnumerable<IssuingStock>> GetAllActiveIssuingStock();
        Task<IEnumerable<IssuingStock>> GetAllIssuingStock();
    }
}
