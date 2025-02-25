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
        Task<Convertionrate> GetConvertionrateById(int id);
        Task<int?> CreateConvertionrate(Convertionrate convertionrate);
        Task<string> UpdateConvertionrate(Convertionrate convertionrate);
        Task<Convertionrate> GetLatestConvertionrateByUOC(string currency);
        Task<List<Convertionrate>> GetAllLatestConvertionrate(SearchParames searchParams);
    }
}
