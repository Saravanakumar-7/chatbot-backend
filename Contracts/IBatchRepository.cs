using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBatchRepository : IRepositoryBase<Batch>
    {
        Task<IEnumerable<Batch>> GetAllBatches(SearchParames searchParams);
        Task<Batch> GetBatchById(int id);
        Task<IEnumerable<Batch>> GetAllActiveBatches(SearchParames searchParams);
        Task<int?> CreateBatch(Batch batch);
        Task<string> UpdateBatch(Batch batch);
        Task<string> DeleteBatch(Batch batch);
    }
}
