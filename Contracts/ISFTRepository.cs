using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface ISFTRepository : IRepositoryBase<SFT>
    {
        Task<IEnumerable<SFT>> GetAllSFT();
        Task<SFT> GetSFTById(int id);
        Task<IEnumerable<SFT>> GetAllActiveSFT();
        Task<int?> CreateSFT(SFT sFt);
        Task<string> UpdateSFT(SFT sFt);
        Task<string> DeleteSFT(SFT sFt);
    }
}