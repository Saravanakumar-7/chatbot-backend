using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IReturnGrinRepository : IRepositoryBase<ReturnGrin>
    {
        Task<ReturnGrin> CreateReturnGrin(ReturnGrin returnGrin);
        Task<string> UpdateReturnGrin(ReturnGrin returnGrin);
        Task<string> DeleteReturnGrin(ReturnGrin returnGrin);

        Task<PagedList<ReturnGrin>> GetAllReturnGrin(PagingParameter pagingParameter);

        Task<ReturnGrin> GetReturnGrinDetailsbyId(int id);
        Task<IEnumerable<ReturnGrinPartsListDto>> ReturnGrinPartsByPartNumber(string partNo);

        public void SaveAsync();
    }
}
