using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinRepository : IRepositoryBase<OpenGrin>
    {
        Task<PagedList<OpenGrin>> GetAllOpenGrinDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<string> UpdateOpenGrin(OpenGrin openGrin);
        Task<OpenGrin> GetOpenGrinDetailsbyId(int id);
        Task<OpenGrin> CreateOpenGrin(OpenGrin openGrin);
        Task<string> DeleteOpenGrin(OpenGrin openGrin);
        Task<OpenGrinDetails> GetOpenGrinPartDetailsbyId(int id);
        Task<OpenGrinParts> GetOpenGrinPartsbyId(int id);
    }
}
