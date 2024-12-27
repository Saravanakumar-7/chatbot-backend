using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface I_SA_Weighted_AvgCostRepository : IRepositoryBase<SA_Weighted_AvgCost>
    {
        Task<List<SA_Weighted_AvgCost>> GetAllSA_Weighted_AvgCost();
        Task DeleteExistingData();
        Task<Dictionary<string, decimal>> GetSAsAndLatestVersion();
        Task<EnggBom> GetEnggBomByItemNoAndRevNo(string itemNumber, decimal revisionNumber);
        Task<SA_Weighted_AvgCost?> GetSA_Weighted_AvgCost(string Itemnumber);
        Task<WeightedAvgRate> GetPPWeightedAvgCost(string Itemnumber);
        void CreateSA_Weighted_AvgCost(SA_Weighted_AvgCost sA_Weighted_AvgCost);
    }
}
