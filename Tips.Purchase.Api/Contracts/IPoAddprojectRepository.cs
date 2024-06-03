using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoAddprojectRepository : IRepositoryBase<PoAddProject>
    {
        Task<IEnumerable<PoAddProject>> GetAllPoAddprojects();
        Task<PoAddProject> GetPoAddprojectById(int id);
        Task<IEnumerable<PoAddProject>> GetAllActivePoAddprojects();
        Task<int?> CreatePoAddproject(PoAddProject poAddproject);
        Task<string> UpdatePoAddproject(PoAddProject poAddproject);
        Task<string> DeletePoAddproject(PoAddProject poAddproject);
        Task<IEnumerable<PoAddProject>> GetPOProjectNoDetailsByProjectNo(string itemNumber, string projectNo, int poItemId);
    }
}
