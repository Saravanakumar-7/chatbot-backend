using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrAddprojectRepository
    {
        Task<IEnumerable<PrAddProject>> GetAllPrAddproject();
        Task<PrAddProject> GetPrAddprojectById(int id);
        Task<IEnumerable<PrAddProject>> GetAllActivePrAddproject();
        Task<int?> CreatePrAddproject(PrAddProject prAddProject);
        Task<string> UpdatePrAddproject(PrAddProject prAddProject);
        Task<string> DeletePrAddproject(PrAddProject prAddProject);
    }
}
