using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCustomerSupportNotesRepository
    {
        Task<IEnumerable<ForeCastCustomerSupportNotes>> GetAllForeCastNotes();
        Task<ForeCastCustomerSupportNotes> GetForeCastNotesById(int id);
        Task<int?> CreateForeCastNote(ForeCastCustomerSupportNotes foreCastNotes);
        Task<string> UpdateForeCastNote(ForeCastCustomerSupportNotes foreCastNotes);
        Task<string> DeleteForeCastNote(ForeCastCustomerSupportNotes foreCastNotes);
    }
}
