using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqCustomerSupportNotesRepository 
    {
        Task<IEnumerable<RfqCustomerSupportNotes>> GetAllRfqNotes();
        Task<RfqCustomerSupportNotes> GetRfqNotesById(int id);
        Task<int?> CreateRfqNotes(RfqCustomerSupportNotes rfqNotes);
        Task<string> UpdateRfqNotes(RfqCustomerSupportNotes rfqNotes);
        Task<string> DeleteRfqNotes(RfqCustomerSupportNotes rfqNotes);
    }
}
