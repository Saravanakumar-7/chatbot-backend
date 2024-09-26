using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface I_SA_Weighted_AvgCost_History_Repository
    {
        Task TranferToSAWeightedHistory(List<SA_Weighted_AvgCost_History> sA_Weighted_AvgCost_Histories);
    }
}
