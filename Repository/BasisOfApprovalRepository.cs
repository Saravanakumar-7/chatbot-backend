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
    public class BasisOfApprovalRepository : RepositoryBase<BasisOfApproval>, IBasisOfApprovalRepository

    {
        public BasisOfApprovalRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            basisOfApproval.CreatedBy = "Admin";
            basisOfApproval.CreatedOn = DateTime.Now;
            var result = await Create(basisOfApproval);
            basisOfApproval.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            Delete(basisOfApproval);
            string result = $"Basis Of Approval details of {basisOfApproval.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<BasisOfApproval>> GetAlBasisOfApproval()
        {
            var basisOfApprovals = await FindAll().ToListAsync();

            return basisOfApprovals;
        }

        public async Task<IEnumerable<BasisOfApproval>> GetAllActiveBasisOfApproval()
        {
            var basisOfApprovals = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return basisOfApprovals;
        }

        public async Task<BasisOfApproval> GetBasisOfApprovalById(int id)
        {
            var basisOfApproval = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return basisOfApproval;
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
