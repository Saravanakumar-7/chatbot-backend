using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface I_FG_Weighted_AvgCostRepository : IRepositoryBase<FG_Weighted_AvgCost>
    {
        Task<List<FG_Weighted_AvgCost>> GetAllFG_Weighted_AvgCost();
        Task DeleteExistingData();
        Task<FG_Weighted_AvgCost?> GetFG_Weighted_AvgCost(string Itemnumber);
        Task<WeightedAvgRate> GetPPWeightedAvgCost(string Itemnumber);
        Task CreateFG_Weighted_AvgCost(FG_Weighted_AvgCost fG_Weighted_AvgCost);
        Task<List<FG_Weighted_AvgCost_Report>> FG_Weighted_AvgCost_Report_withParameter(string FGItemNumber);
        Task<List<FG_Weighted_AvgCost_Report_withDate>> FG_Weighted_AvgCost_Report_withDate(string FromDate, string ToDate);
        Task<List<Weighted_AvgCost_Report>> Weighted_AvgCost_Report_withParameter(string? FGItemNumber);
    }
}
