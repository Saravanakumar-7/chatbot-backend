using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Entities.DTOs;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPOBreakDownRepository : IRepositoryBase<POBreakDown>
    {
        Task<IEnumerable<POBreakDown>> GetAllPOBreakDown();
        Task<POBreakDown> GetPOBreakDownById(int id);
        Task<int?> CreatePOBreakDown(POBreakDown pocollectionTrackerItem);
        Task<string> UpdatePOBreakDown(POBreakDown pocollectionTrackerItem);
        Task<string> DeletePOBreakDown(POBreakDown pocollectionTrackerItem);
    }
}

