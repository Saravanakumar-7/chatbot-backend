using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IHSNRepository
    {
        Task<IEnumerable<Hsn>> GetAllHSN(SearchParames searchParams);
        Task<Hsn> GetHSNById(int id);
        Task<IEnumerable<Hsn>> GetAllActiveHSN(SearchParames searchParams);
        Task<int?> CreateHSN(Hsn hsn);
        Task<string> UpdateHSN(Hsn hsn);
        Task<string> DeleteHSN(Hsn hsn);
    }
}
