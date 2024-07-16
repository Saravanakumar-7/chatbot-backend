using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinForIQCRepository : IRepositoryBase<OpenGrinForIQC>
    {
        Task<PagedList<OpenGrinForIQC>> GetAllOpenGrinForIQCDetails(PagingParameter pagingParameter,SearchParams searchParams);
        Task<int?> CreateOpenGrinForIQC(OpenGrinForIQC openGrinForIQC);
        Task<OpenGrinForIQC> GetOpenGrinForIQCDetailsbyId(int id);
        Task<OpenGrinForIQC> GetOpenGrinForIQCDetailsbyOpenGrinNo(string openGrinNumber);
        Task<string> UpdateOpenGrinForIQC(OpenGrinForIQC openGrinForIQC);
    }
}
