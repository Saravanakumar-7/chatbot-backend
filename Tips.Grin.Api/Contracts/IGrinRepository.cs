using Entities; 
using Tips.Grin.Api.Entities.DTOs;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.Grin.Api.Entities; 

namespace Tips.Grin.Api.Contracts
{
    public interface IGrinRepository : IRepositoryBase<Grins>
    {
        Task<PagedList<Grins>> GetAllGrin(PagingParameter pagingParameter);
        Task<Grins> GetGrinById(int id);
        Task<IEnumerable<Grins>> GetAllActiveGrin();
        Task<int?> CreateGrin(Grins grins);
        Task<string> UpdateGrin(Grins grins);
        Task<string> DeleteGrin(Grins grins);

        Task<IEnumerable<GrinNoListDto>> GetAllActiveGrinNoList();
    }
}
