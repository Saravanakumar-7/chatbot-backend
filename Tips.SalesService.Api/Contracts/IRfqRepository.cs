using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
 
    public interface IRfqRepository
    {
        Task<IEnumerable<Rfq>> GetAllRfq();
        Task<Rfq> GetRfqById(int id);
        Task<int?> CreateRfq(Rfq rfq);
        Task<string> UpdateRfq(Rfq rfq);
        Task<string> DeleteRfq(Rfq rfq);
    } 

}
