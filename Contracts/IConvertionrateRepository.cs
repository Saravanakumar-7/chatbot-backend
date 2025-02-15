using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IConvertionrateRepository : IRepositoryBase<Convertionrate>
    {
        Task<IEnumerable<Convertionrate>> GetAllConvertionrate(SearchParames searchParams);
        Task<Convertionrate> GetConvertionrateById(int id);
        Task<IEnumerable<Convertionrate>> GetAllActiveConvertionrate(SearchParames searchParams);
        Task<int?> CreateConvertionrate(Convertionrate convertionrate);
        Task<string> UpdateConvertionrate(Convertionrate convertionrate);
        Task<string> DeleteConvertionrate(Convertionrate convertionrate);
        Task<Convertionrate> GetLatestConvertionrateByUOC(string currency);
        Task<List<Convertionrate>> GetAllLatestConvertionrate();
    }
}
