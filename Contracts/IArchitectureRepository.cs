using Entities;
using Entities.DTOs;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IArchitectureRepository : IRepositoryBase<Architectures>
    {
        Task<IEnumerable<Architectures>> GetAllArchitectures(SearchParames searchParams);

        Task<IEnumerable<Architectures>> GetAllArchitecturesDetails();

        Task<Architectures> GetArchitectureById(int id);
        Task<IEnumerable<Architectures>> GetAllActiveArchitectures(SearchParames searchParams);
        Task<int?> CreateArchitecture(Architectures architecture);
        Task<string> UpdateArchitecture(Architectures architecture);
        Task<string> DeleteArchitecture(Architectures architecture);
    }
}