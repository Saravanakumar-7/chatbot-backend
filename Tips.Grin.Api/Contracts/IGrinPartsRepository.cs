using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Contracts
{
    public interface IGrinPartsRepository
    {
        Task<IEnumerable<GrinParts>> GetAllGrinParts();
        Task<GrinParts> GetGrinPartsById(int id);
        Task<IEnumerable<GrinParts>> GetAllActiveGrinParts();
        Task<int?> CreateGrinParts(GrinParts grinParts);
        Task<string> UpdateGrinParts(GrinParts grinParts);
        Task<string> DeleteGrinParts(GrinParts grinParts);
    }
}
