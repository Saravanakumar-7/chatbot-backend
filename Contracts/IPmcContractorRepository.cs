using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IPmcContractorRepository : IRepositoryBase<PmcContractor>
    {
        Task<IEnumerable<PmcContractor>> GetAllPmcContractor();
        Task<PmcContractor> GetPmcContractorById(int id);
        Task<IEnumerable<PmcContractor>> GetAllActivePmcContractor();
        Task<int?> CreatePmcContractor(PmcContractor pmcContractor);
        Task<string> UpdatePmcContractor(PmcContractor pmcContractor);
        Task<string> DeletePmcContractor(PmcContractor pmcContractor);
    }
}