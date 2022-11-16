using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IRiskCategoryRepository : IRepositoryBase<RiskCategory>
    {
        Task<IEnumerable<RiskCategory>> GetAllRiskCategory();
        Task<RiskCategory> GetRiskCategoryById(int id);
        Task<IEnumerable<RiskCategory>> GetAllActiveRiskCategory();
        Task<int?> CreateRiskCategory(RiskCategory riskCategory);
        Task<string> UpdateRiskCategory(RiskCategory riskCategory);
        Task<string> DeleteRiskCategory(RiskCategory riskCategory);
    }
}
