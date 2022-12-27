using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EngineeringBomRepository : RepositoryBase<EnggBom>, IEnggBomRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public EngineeringBomRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
        }

        public async Task<int?> CreateEnggBom(EnggBom enggBom)
        {
            enggBom.CreatedBy = "Admin";
            enggBom.CreatedOn = DateTime.Now;
            enggBom.LastModifiedBy = "Admin";
            enggBom.LastModifiedOn = DateTime.Now;
            enggBom.Unit = "Bangalore";

            var result = await Create(enggBom);
            return result.BOMId;
        }

        public async Task<string> DeleteEnggBom(EnggBom enggBom)
        {
            Delete(enggBom);
            string result = $"BOM details of {enggBom.BOMId} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<EnggBom>> GetAllActiveEnggBom()
        {
            var bomDetails = await FindAll().ToListAsync();
            return bomDetails;
        }

        public async Task<PagedList<EnggBom>> GetAllEnggBOM(PagingParameter pagingParameter)
        {


            var bomDetails = PagedList<EnggBom>.ToPagedList(FindAll()
               .OrderBy(on => on.BOMId), pagingParameter.PageNumber, pagingParameter.PageSize);


            return bomDetails;
        }

        public async Task<EnggBom> GetEnggBomById(int id)
        {
            var bomDetails = await _tipsMasterDbContext.EnggBoms.Where(x => x.BOMId == id)
                              .Include(t => t.EnggChildItems)
                                .ThenInclude(x => x.EnggAlternates)
                                .Include(m => m.EnggChildItems)
                                .ThenInclude(i => i.NREConsumable)
                              .FirstOrDefaultAsync();

            return bomDetails;
        }

        public async Task<string> UpdateEnggBom(EnggBom enggBom)
        {
            enggBom.LastModifiedBy = "Admin";
            enggBom.LastModifiedOn = DateTime.Now;
            Update(enggBom);
            string result = $"Engineering BOM Detail {enggBom.BOMId} is updated successfully!";
            return result;
        }
    }

    public class ReleaseEnggBomRepository : RepositoryBase<ReleaseEnggBom>, IReleaseEnggBomRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public ReleaseEnggBomRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
        }

        public async Task<int?> CreateReleaseEnggBom(ReleaseEnggBom releaseEnggBom)
        {
            releaseEnggBom.CreatedBy = "Admin";
            releaseEnggBom.CreatedOn = DateTime.Now;
            releaseEnggBom.LastModifiedBy = "Admin";
            releaseEnggBom.LastModifiedOn = DateTime.Now;
            var result = await Create(releaseEnggBom);
            return result.Id;
        }

        public async Task<string> DeleteReleaseEnggBom(ReleaseEnggBom releaseEnggBom)
        {
            Delete(releaseEnggBom);
            string result = $"ReleaseEnggBom details of {releaseEnggBom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ReleaseEnggBom>> GetAllActiveReleaseEnggBom()
        {
            var releaseEnggBomDetails = await FindAll().ToListAsync();
            return releaseEnggBomDetails;
        }

        public async Task<PagedList<ReleaseEnggBom>> GetAllReleaseEnggBom(PagingParameter pagingParameter)
        {
            var releaseEnggBomDetails = PagedList<ReleaseEnggBom>.ToPagedList(FindAll()
              .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);


            return releaseEnggBomDetails;
        }

        public async Task<ReleaseEnggBom> GetReleaseEnggBomById(int id)
        {
            var releaseEnggBomDetails = await _tipsMasterDbContext.ReleaseEnggBoms.Where(x => x.Id == id).FirstOrDefaultAsync();
            return releaseEnggBomDetails;
        }

        public async Task<string> UpdateReleaseEnggBom(ReleaseEnggBom releaseEnggBom)
        {
            releaseEnggBom.LastModifiedBy = "Admin";
            releaseEnggBom.LastModifiedOn = DateTime.Now;
            Update(releaseEnggBom);
            string result = $"ReleaseEnggBom Detail {releaseEnggBom.Id} is updated successfully!";
            return result;
        }
    }

    public class ReleaseCostBomRepository : RepositoryBase<ReleaseCostBom>, IReleaseCostBomRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public ReleaseCostBomRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
        }

        public async Task<int?> CreateReleaseCostBom(ReleaseCostBom releaseCostBom)
        {
            releaseCostBom.CreatedBy = "Admin";
            releaseCostBom.CreatedOn = DateTime.Now;
            releaseCostBom.LastModifiedBy = "Admin";
            releaseCostBom.LastModifiedOn = DateTime.Now;
            var result = await Create(releaseCostBom);
            return result.Id;
        }
    }

    public class ReleaseProductBomRepository : RepositoryBase<ReleaseProductBom>, IReleaseProductBomRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public ReleaseProductBomRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
        }

        public async Task<int?> CreateReleaseProductBom(ReleaseProductBom releaseProductBom)
        {
            releaseProductBom.CreatedBy = "Admin";
            releaseProductBom.CreatedOn = DateTime.Now;
            releaseProductBom.LastModifiedBy = "Admin";
            releaseProductBom.LastModifiedOn = DateTime.Now;
            var result = await Create(releaseProductBom);
            return result.Id;
        }
    }
    public class EnggBomGroupRepository : RepositoryBase<EnggBomGroup>, IEnggBomGroupRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public EnggBomGroupRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
        }

        public async Task<int?> CreateEnggBomGroup(EnggBomGroup enggbomGroup)
        {
            enggbomGroup.CreatedBy = "Admin";
            enggbomGroup.CreatedOn = DateTime.Now;
            enggbomGroup.LastModifiedBy = "Admin";
            enggbomGroup.LastModifiedOn = DateTime.Now;
            var result = await Create(enggbomGroup);
            return result.Id;
        }

        public async Task<string> DeleteEnggBomGroup(EnggBomGroup enggbomGroup)
        {
            Delete(enggbomGroup);
            string result = $"EnggBomGroup details of {enggbomGroup.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<EnggBomGroup>> GetAllActiveEnggBomGroup()
        {
            var enggbomGroupDetails = await FindAll().ToListAsync();
            return enggbomGroupDetails;
        }

        public async Task<PagedList<EnggBomGroup>> GetAllEnggBomGroup(PagingParameter pagingParameter)
        {
            var enggbomGroupDetails = PagedList<EnggBomGroup>.ToPagedList(FindAll()
             .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);


            return enggbomGroupDetails;
        }

        public async Task<EnggBomGroup> GetEnggBomGroupById(int id)
        {
            var enggbomGroupDetails = await _tipsMasterDbContext.BomGroups.Where(x => x.Id == id).FirstOrDefaultAsync();
            return enggbomGroupDetails;
        }

        public async Task<string> UpdateEnggBomGroup(EnggBomGroup enggbomGroup)
        {
            enggbomGroup.LastModifiedBy = "Admin";
            enggbomGroup.LastModifiedOn = DateTime.Now;
            Update(enggbomGroup);
            string result = $"EnggBomGroup Detail {enggbomGroup.Id} is updated successfully!";
            return result;
        }
    }
    public class EnggCustomFieldRepository : RepositoryBase<EnggCustomField>, IEnggCustomFieldRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public EnggCustomFieldRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
        }

        public async Task<int?> CreateEnggCustomField(EnggCustomField enggcustomFields)
        {
            enggcustomFields.CreatedBy = "Admin";
            enggcustomFields.CreatedOn = DateTime.Now;
            enggcustomFields.LastModifiedBy = "Admin";
            enggcustomFields.LastModifiedOn = DateTime.Now;
            var result = await Create(enggcustomFields);
            return result.Id;
        }

        public async Task<string> DeleteEnggCustomField(EnggCustomField enggcustomFields)
        {
            Delete(enggcustomFields);
            string result = $"EnggCustomFields details of {enggcustomFields.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<EnggCustomField>> GetAllActiveEnggCustomFields()
        {
            var enggcustomFieldsDetails = await FindAll().ToListAsync();
            return enggcustomFieldsDetails;
        }

        public async Task<PagedList<EnggCustomField>> GetAllEnggCustomFields(PagingParameter pagingParameter)
        {
            var enggcustomFieldsDetails = PagedList<EnggCustomField>.ToPagedList(FindAll()
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return enggcustomFieldsDetails;
        }

        public async Task<EnggCustomField> GetEnggCustomFieldById(int id)
        {
            var enggcustomFieldsDetails = await _tipsMasterDbContext.CustomFields.Where(x => x.Id == id).FirstOrDefaultAsync();
            return enggcustomFieldsDetails;
        }

        public async Task<string> UpdateEnggCustomField(EnggCustomField enggcustomFields)
        {
            enggcustomFields.LastModifiedBy = "Admin";
            enggcustomFields.LastModifiedOn = DateTime.Now;
            Update(enggcustomFields);
            string result = $"EnggCustomFields Detail {enggcustomFields.Id} is updated successfully!";
            return result;
        }
    }
}