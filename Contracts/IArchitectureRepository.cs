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
        Task<PagedList<Architectures>> GetAllArchitectures(PagingParameter pagingParameter, SearchParames searchParams);
        Task<Architectures> GetArchitectureById(int id);
        Task<PagedList<Architectures>> GetAllActiveArchitectures(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateArchitecture(Architectures architecture);
        Task<string> UpdateArchitecture(Architectures architecture);
        Task<string> DeleteArchitecture(Architectures architecture);
    }
}