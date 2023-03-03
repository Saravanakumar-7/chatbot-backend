using Entities;
using Entities.DTOs;
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
        Task<IEnumerable<Architectures>> GetAllArchitectures();
        Task<Architectures> GetArchitectureById(int id);
        Task<IEnumerable<Architectures>> GetAllActiveArchitectures();
        Task<int?> CreateArchitecture(Architectures architecture);
        Task<string> UpdateArchitecture(Architectures architecture);
        Task<string> DeleteArchitecture(Architectures architecture);
    }
}