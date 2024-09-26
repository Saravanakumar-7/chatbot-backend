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
        Task<IEnumerable<ItemmasterAlternate>> GetAllItemmasterAlternates();
        Task<ItemmasterAlternate> GetItemmasterAlternateById(int id);
        Task<IEnumerable<ItemmasterAlternate>> GetAllActiveItemmasterAlternates();
        Task<int?> CreateItemmasterAlternate(ItemmasterAlternate itemmasterAlternate);
        Task<string> UpdateItemmasterAlternate(ItemmasterAlternate itemmasterAlternate);
        Task<string> DeleteItemmasterAlternate(ItemmasterAlternate itemmasterAlternate);
    }
}
