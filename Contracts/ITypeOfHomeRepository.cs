using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface ITypeOfHomeRepository : IRepositoryBase<TypeOfHome>
    {
        Task<IEnumerable<TypeOfHome>> GetAllTypeOfHome();
        Task<TypeOfHome> GetTypeOfHomeById(int id);
        Task<IEnumerable<TypeOfHome>> GetAllActiveTypeOfHome();
        Task<int?> CreateTypeOfHome(TypeOfHome typeOfHome);
        Task<string> UpdateTypeOfHome(TypeOfHome typeOfHome);
        Task<string> DeleteTypeOfHome(TypeOfHome typeOfHome);
    }
}