using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SA_Weighted_AvgCost_History_Repository : RepositoryBase<SA_Weighted_AvgCost_History>, I_SA_Weighted_AvgCost_History_Repository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SA_Weighted_AvgCost_History_Repository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task TranferToSAWeightedHistory(List<SA_Weighted_AvgCost_History> sA_Weighted_AvgCost_Histories)
        {
            foreach (var SA in sA_Weighted_AvgCost_Histories)
            {
                SA.Id = 0;
                await Create(SA);
            }
        }
    }
}
