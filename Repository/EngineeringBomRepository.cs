using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
            var AllActiveEnggBomDetails = await FindAll().ToListAsync();
            return AllActiveEnggBomDetails;
        }

        public async Task<PagedList<EnggBom>> GetAllEnggBOM(PagingParameter pagingParameter)
        {


            var GetallEnggbomDetails = PagedList<EnggBom>.ToPagedList(FindAll()
               .OrderByDescending(x => x.BOMId), pagingParameter.PageNumber, pagingParameter.PageSize);


            return GetallEnggbomDetails;
        }

        public async Task<EnggBom> GetEnggBomByFgPartNumber(string fgPartNumber)
        {
            var EnggBomDetailsbyId = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == fgPartNumber)                              
                                .Include(m => m.NREConsumable)
                                .Include(t => t.EnggChildItems)
                                .ThenInclude(x => x.EnggAlternates)
                              .FirstOrDefaultAsync();

            return EnggBomDetailsbyId;
        }

        public async Task<EnggBom> GetEnggBomById(int id)
        {
            var EnggBomDetailsbyId = await _tipsMasterDbContext.EnggBoms.Where(x => x.BOMId == id)                                                           
                                .Include(m => m.NREConsumable)
                                .Include(t => t.EnggChildItems)
                                .ThenInclude(x => x.EnggAlternates)
                              .FirstOrDefaultAsync();

            return EnggBomDetailsbyId;
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
            var AllActiveReleaseEnggBomDetails = await FindAll().ToListAsync();
            return AllActiveReleaseEnggBomDetails;
        }

        public async Task<PagedList<ReleaseEnggBom>> GetAllReleaseEnggBom(PagingParameter pagingParameter)
        {
            var GetallReleaseEnggBomDetails = PagedList<ReleaseEnggBom>.ToPagedList(FindAll()
              .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);


            return GetallReleaseEnggBomDetails;
        }

        public async Task<ReleaseEnggBom> GetReleaseEnggBomById(int id)
        {
            var ReleaseEnggBomDetailsbyId = await _tipsMasterDbContext.ReleaseEnggBoms.Where(x => x.Id == id).FirstOrDefaultAsync();
            return ReleaseEnggBomDetailsbyId;
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
            var AllActiveEnggbomGroupDetails = await FindAll().ToListAsync();
            return AllActiveEnggbomGroupDetails;
        }

        public async  Task<IEnumerable<ListOfBomGroupDto>> GetAllBomGroupList()
        {
            IEnumerable<ListOfBomGroupDto> getAllBomGroupList = await _tipsMasterDbContext.BomGroups
                               .Select(c => new ListOfBomGroupDto()
                               {
                                   Id = c.Id,
                                   BomGroupName = c.BomGroupName,

                               })
                               .OrderByDescending(c => c.Id)
                             .ToListAsync();

            return getAllBomGroupList;
        }

        public async Task<PagedList<EnggBomGroup>> GetAllEnggBomGroup(PagingParameter pagingParameter)
        {
            var GetallEnggbomGroupDetails = PagedList<EnggBomGroup>.ToPagedList(FindAll()
             .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);


            return GetallEnggbomGroupDetails;
        }

        public async Task<EnggBomGroup> GetEnggBomGroupById(int id)
        {
            var EnggbomGroupDetailsbyId = await _tipsMasterDbContext.BomGroups.Where(x => x.Id == id).FirstOrDefaultAsync();
            return EnggbomGroupDetailsbyId;
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
            var AllActiveEnggcustomFieldsDetails = await FindAll().ToListAsync();
            return AllActiveEnggcustomFieldsDetails;
        }

        public async Task<PagedList<EnggCustomField>> GetAllEnggCustomFields(PagingParameter pagingParameter)
        {
            var GetallEnggcustomFieldsDetails = PagedList<EnggCustomField>.ToPagedList(FindAll()
            .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return GetallEnggcustomFieldsDetails;
        }

        public async Task<IEnumerable<EnggCustomField>> GetEnggCustomFieldByBomGroup(string BomgroupName)
        {
            var getEnggCustomFieldByBomGroup = await FindByCondition(x => x.BOMGroupName == BomgroupName).ToListAsync();

            return getEnggCustomFieldByBomGroup;
        }

        public async Task<EnggCustomField> GetEnggCustomFieldById(int id)
        {
            var EnggcustomFieldsDetailsbyId = await _tipsMasterDbContext.CustomFields.Where(x => x.Id == id).FirstOrDefaultAsync();
            return EnggcustomFieldsDetailsbyId;
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

    public class EngineeringNREConsumableRepository : RepositoryBase<NREConsumable>, IEnggBomNREConsumableRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public EngineeringNREConsumableRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
        }

        public Task<int?> CreateEnggNREConsumable(NREConsumable bomNREConsumable)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteEnggNREConsumable(NREConsumable bomNREConsumable)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<NREConsumable>> GetAllActiveEnggNREConsumable()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<NREConsumable>> GetAllEnggNREConsumable()
        {
            throw new NotImplementedException();
        }

        public async Task<NREConsumable> GetAllNREConsumableLists(int id)
        {
            var getRountingList = await _tipsMasterDbContext.BomNREConsumables
                                   .Where(x => x.EnggBomId == id).FirstOrDefaultAsync();
            return getRountingList;
        }
         

        public Task<NREConsumable> GetEnggNREConsumableById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateEnggNREConsumable(NREConsumable bomNREConsumable)
        {
            throw new NotImplementedException();
        }
    }

}