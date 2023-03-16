using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IItemmasterAlternate
    {
        Task<PagedList<ItemmasterAlternate>> GetAllItemmasterAlternates(PagingParameter pagingParameter, SearchParames searchParames);
        Task<ItemmasterAlternate> GetItemmasterAlternateById(int id);
        Task<PagedList<ItemmasterAlternate>> GetAllActiveItemmasterAlternates(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateItemmasterAlternate(ItemmasterAlternate itemmasterAlternate);
        Task<string> UpdateItemmasterAlternate(ItemmasterAlternate itemmasterAlternate);
        Task<string> DeleteItemmasterAlternate(ItemmasterAlternate itemmasterAlternate);
    }
}
