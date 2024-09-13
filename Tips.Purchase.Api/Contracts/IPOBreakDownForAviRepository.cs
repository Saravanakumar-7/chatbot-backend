using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPOBreakDownForAviRepository : IRepositoryBase<POBreakDownForAvi>
    {
        Task<IEnumerable<POBreakDownForAvi>> GetAllPOBreakDownForAvi();
        Task<POBreakDownForAvi> GetPOBreakDownForAviById(int id);
        Task<int?> CreatePOBreakDownForAvi(POBreakDownForAvi pocollectionTrackerItem);
        Task<string> UpdatePOBreakDownForAvi(POBreakDownForAvi pocollectionTrackerItem);
        Task<string> DeletePOBreakDownForAvi(POBreakDownForAvi pocollectionTrackerItem);
    }
}
