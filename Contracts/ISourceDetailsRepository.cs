using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface ISourceDetailsRepository : IRepositoryBase<SourceDetails>
    {
        Task<IEnumerable<SourceDetails>> GetAllSourceDetails();
        Task<SourceDetails> GetSourceDetailsById(int id);
        Task<IEnumerable<SourceDetails>> GetAllActiveSourceDetails();
        Task<int?> CreateSourceDetails(SourceDetails sourceDetails);
        Task<string> UpdateSourceDetails(SourceDetails sourceDetails);
        Task<string> DeleteSourceDetails(SourceDetails sourceDetails);
    }
}