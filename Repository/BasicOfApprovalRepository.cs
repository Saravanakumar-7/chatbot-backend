using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BasicOfApprovalRepository : RepositoryBase<BasicOfApproval>, IBasicOfApprovalRepository

    {
        public BasicOfApprovalRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateBasicOfApproval(BasicOfApproval basicOfApproval)
        {
            basicOfApproval.CreatedBy = "Admin";
            basicOfApproval.CreatedOn = DateTime.Now;
            var result = await Create(basicOfApproval);
            return result.Id;
        }

        public async Task<string> DeleteBasicOfApproval(BasicOfApproval basicOfApproval)
        {
            Delete(basicOfApproval);
            string result = $"Basic Of Approval details of {basicOfApproval.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<BasicOfApproval>> GetAlBasicOfApproval()
        {
            var basicOfApprovals = await FindAll().ToListAsync();

            return basicOfApprovals;
        }

        public async Task<IEnumerable<BasicOfApproval>> GetAllActiveBasicOfApproval()
        {
            var basicOfApprovals = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return basicOfApprovals;
        }

        public async Task<BasicOfApproval> GetBasicOfApprovalById(int id)
        {
            var basicOfApproval = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return basicOfApproval;
        }

        public async Task<string> UpdateBasicOfApproval(BasicOfApproval basicOfApproval)
        {
            basicOfApproval.LastModifiedBy = "Admin";
            basicOfApproval.LastModifiedOn = DateTime.Now;
            Update(basicOfApproval);
            string result = $"Basic Of Approval of Detail {basicOfApproval.Id} is updated successfully!";
            return result;
        }
         
    }
}
