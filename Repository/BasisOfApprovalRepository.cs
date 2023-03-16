using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BasisOfApprovalRepository : RepositoryBase<BasisOfApproval>, IBasisOfApprovalRepository

    {
        public BasisOfApprovalRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            basisOfApproval.CreatedBy = "Admin";
            basisOfApproval.CreatedOn = DateTime.Now;
            basisOfApproval.Unit = "Bangalore";
            var result = await Create(basisOfApproval);
            return result.Id;
        }

        public async Task<string> DeleteBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            Delete(basisOfApproval);
            string result = $"Basis Of Approval details of {basisOfApproval.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<BasisOfApproval>> GetAllBasisOfApproval([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var basisOfApprovalDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BasisOfApprovalName.Contains(searchParams.SearchValue) ||
                inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<BasisOfApproval>.ToPagedList(basisOfApprovalDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<BasisOfApproval>> GetAllActiveBasisOfApproval([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var getAllActiveBasisOfApprovals = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BasisOfApprovalName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<BasisOfApproval>.ToPagedList(getAllActiveBasisOfApprovals, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<BasisOfApproval> GetBasisOfApprovalById(int id)
        {
            var BasisOfApprovalbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return BasisOfApprovalbyId;
        }

        public async Task<string> UpdateBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            basisOfApproval.LastModifiedBy = "Admin";
            basisOfApproval.LastModifiedOn = DateTime.Now;
            Update(basisOfApproval);
            string result = $"Basis Of Approval of Detail {basisOfApproval.Id} is updated successfully!";
            return result;
        }
         
    }
}
