using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface I_FG_Weighted_AvgCost_History_Repository
    {
        Task TranferToFGWeightedHistory(List<FG_Weighted_AvgCost_History> fG_Weighted_AvgCost_Histories);
    }
}
