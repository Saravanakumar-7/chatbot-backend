using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Grin.Api.Contracts
{
    public interface IBinningItemsRepository : IRepositoryBase<BinningItems>
    {
        Task<IEnumerable<BinningItems>> GetAllBinningItems();

    }
}
