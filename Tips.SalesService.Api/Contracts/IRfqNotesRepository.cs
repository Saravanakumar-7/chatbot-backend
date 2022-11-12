using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqNotesRepository
    {
        Task<IEnumerable<RfqNotes>> GetAllRfqNotes();
        Task<RfqNotes> GetRfqNotesById(int id);
        Task<int?> CreateRfqNotes(RfqNotes rfqNotes);
        Task<string> UpdateRfqNotes(RfqNotes rfqNotes);
        Task<string> DeleteRfqNotes(RfqNotes rfqNotes);
    }
}
