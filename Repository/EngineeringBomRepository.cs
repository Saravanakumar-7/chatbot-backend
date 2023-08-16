using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EngineeringBomRepository : RepositoryBase<EnggBom>, IEnggBomRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private TipsMasterDbContext _tipsMasterDbContext;
        public EngineeringBomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

            _tipsMasterDbContext = repositoryContext;
        }

        public async Task<EnggBom> UpdateEnggBomVersion(EnggBom enggBom)
        {
            enggBom.CreatedBy = _createdBy;
            enggBom.CreatedOn = DateTime.Now;
            enggBom.Unit = _unitname;
            var getOldRevisionNumber = _tipsMasterDbContext.EnggBoms
                .Where(x => x.ItemNumber == enggBom.ItemNumber)
                .OrderByDescending(x => x.BOMId)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();

            enggBom.RevisionNumber = getOldRevisionNumber;
            var result = await Create(enggBom);
            return result;

        }

        public async Task<int?> CreateEnggBom(EnggBom enggBom)
        { 
            enggBom.CreatedBy = _createdBy;
            enggBom.CreatedOn = DateTime.Now;
            enggBom.LastModifiedBy = _createdBy;
            enggBom.LastModifiedOn = DateTime.Now;
            enggBom.Unit = _unitname; 
            var result = await Create(enggBom);
            return result.BOMId;
        }
         

        public async Task<string> DeleteEnggBom(EnggBom enggBom)
        {
            Delete(enggBom);
            string result = $"BOM details of {enggBom.BOMId} is deleted successfully!";
            return result;
        }

        //public async Task<PagedList<EnggBom>> GetAllActiveEnggBom([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var enggBomDetails = FindAll()
        //                     .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
        //                        inv.ItemType.Equals(int.Parse(searchParams.SearchValue)) || inv.ItemDescription.Contains(searchParams.SearchValue))))
        //                       .Include(t => t.EnggChildItems)
        //                       .ThenInclude(t => t.EnggAlternates)
        //                        .Include(t => t.NREConsumable);


        //    return PagedList<EnggBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<EnggBom>> GetAllActiveEnggBom()
        {
            var allActiveEnggBom = await FindByCondition(x => x.IsActive == true)
            .Include(t => t.EnggChildItems)
            .ThenInclude(t => t.EnggAlternates)
            .Include(t => t.NREConsumable)
            .ToListAsync();
            return allActiveEnggBom;
        }

        public async Task<PagedList<EnggBom>> GetAllEnggBOM([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var enggBomDetails = FindAll().OrderByDescending(x => x.BOMId)
                          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                             /*inv.ItemType.Equals(int.Parse(searchParams.SearchValue)) ||*/ inv.ItemDescription.Contains(searchParams.SearchValue))))
                                .Include(t => t.EnggChildItems)
                               .ThenInclude(t => t.EnggAlternates) 
                               .Include(t => t.NREConsumable);  

            return PagedList<EnggBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<IEnumerable<EnggBomItemDto>> GetAllEnggBOMItemNumber()
        {
            IEnumerable<EnggBomItemDto> getAllEnggBomItems = await _tipsMasterDbContext.EnggBoms
            .Select(c => new EnggBomItemDto()
            {
                ItemNumber = c.ItemNumber,
            })
           .ToListAsync();
            return getAllEnggBomItems;

        }
         
        public async Task<List<EnggBomFGItemNumberWithQtyDto>> GetFGBomItemsChildDetails(List<string> itemNumberList)
        {
            var itemIdNoList = await TipsMasterDbContext.EnggBoms
                .Where(im => itemNumberList.Contains(im.ItemNumber) && im.IsEnggBomRelease == true)
                .Select(x => new { x.ItemNumber,x.BOMId,x.ItemType }).Distinct().ToListAsync();

            List<int> itemNos = itemIdNoList.Where(x=>x.ItemType == PartType.FG).Select(x=>x.BOMId).Distinct().ToList();
                        
            List<EnggBomFGItemNumberWithQtyDto> result = _tipsMasterDbContext.EnggChildItems
                    .Where(x => itemNos.Contains(x.EnggBomId))
                    .GroupBy(l => new { l.ItemNumber, l.Description })
                    .Select(group => new EnggBomFGItemNumberWithQtyDto
                    {
                        ItemNumber = group.Key.ItemNumber,
                        QtyReq = group.Sum(c => c.Quantity),
                        ItemDescription = group.Key.Description
                    }).ToList();

            return result;
        }

        public async Task<IEnumerable<EnggBomFGItemNumber>> GetAllEnggBomFGItemNoListByItemNumber(string itemNumber)
        {
            List<int>bomDetails = await _tipsMasterDbContext.EnggChildItems
                                .Where(x => x.ItemNumber == itemNumber && x.PartType == PartType.SA || x.PartType == PartType.PurchasePart)
                                .Select(x => x.EnggBomId).Distinct().ToListAsync();

            IEnumerable<EnggBomFGItemNumber> getAllBomGroupList = await _tipsMasterDbContext.EnggBoms
                .Where(x => bomDetails.Contains(x.BOMId) )
                .Select(c => new EnggBomFGItemNumber()
                               { 
                                   ItemNumber = c.ItemNumber,
                                    Description = c.ItemDescription
                                }) 
                             .ToListAsync();

            return getAllBomGroupList;
        }

        //test
        // Define a recursive method to find the parent FG item number for a given SA item number
        public async Task<string> FindParentFgItemNumberRecursive(string saItemNumber)
        {
            // Fetch the EnggBomId of the SA item number
            var saEnggBomId = await _tipsMasterDbContext.EnggChildItems
                .Where(x => x.ItemNumber == saItemNumber)
                .Select(x => x.EnggBomId)
                .FirstOrDefaultAsync();

            //if (saEnggBomId == null)
            //{
            //    // SA item number not found, return null or handle the situation accordingly
            //    return null;
            //}

            // Fetch the parent FG item number using the EnggBomId
            var parentFgItemNumber = await _tipsMasterDbContext.EnggBoms
                .Where(x => x.BOMId == saEnggBomId && x.ItemType == PartType.FG)
                .Select(x => x.ItemNumber)
                .FirstOrDefaultAsync();

            //if (parentFgItemNumber == null)
            //{
            //    // Parent FG item number not found, return null or handle the situation accordingly
            //    return null;
            //}

            // Check if the parent FG item has any child SA items and recursively find their parent FG item numbers
            var childSaItemNumbers = await _tipsMasterDbContext.EnggChildItems
                .Where(x => x.EnggBomId == saEnggBomId && x.PartType == PartType.SA)
                .Select(x => x.ItemNumber)
                .ToListAsync();

            foreach (var childSaItemNumber in childSaItemNumbers)
            {
                var recursiveParentFgItemNumber = await FindParentFgItemNumberRecursive(childSaItemNumber);

                if (recursiveParentFgItemNumber != null)
                {
                     return recursiveParentFgItemNumber;
                }
            }

            // Return the parent FG item number
            return parentFgItemNumber;
        }
        //test2
        public async Task<List<EnggBomFGItemNumber>> GetFgItemsList(string childItemNumber)
        {
            List<EnggBomFGItemNumber> fgItemsList = new List<EnggBomFGItemNumber>();
            await GetFgParentItemNumbers(childItemNumber, fgItemsList);

            return fgItemsList;
        }

        private async Task GetFgParentItemNumbers(string childItemNumber, List<EnggBomFGItemNumber> fgItemsList)
        {
            var saParentItemNumber = await GetSAParentItemNumber(childItemNumber);

            if (saParentItemNumber != null)
            {
                var fgParentItems = await GetFgParentItems(saParentItemNumber);
                fgItemsList.AddRange(fgParentItems);

                foreach (var fgParentItem in fgParentItems)
                {
                    await GetFgParentItemNumbers(fgParentItem.ItemNumber, fgItemsList);
                }
            }
        }

        private async Task<string> GetSAParentItemNumber(string childItemNumber)
        {
            var saParentItem = await _tipsMasterDbContext.EnggBoms
                .FirstOrDefaultAsync(x => x.ItemType == PartType.SA && x.EnggChildItems.Any(item => item.ItemNumber == childItemNumber));

            return saParentItem?.ItemNumber;
        }

        private async Task<List<EnggBomFGItemNumber>> GetFgParentItems(string saItemNumber)
        {

            var items = await _tipsMasterDbContext.EnggChildItems
                .Where(x => x.ItemNumber == saItemNumber && x.PartType == PartType.SA).Select(x => x.EnggBomId).ToListAsync();

            var fgParentItems = await _tipsMasterDbContext.EnggBoms
        .Where(x => items.Contains(x.BOMId) && x.ItemType == PartType.FG)
            .Select(c => new EnggBomFGItemNumber()
                {
                    ItemNumber = c.ItemNumber,
                    Description = c.ItemDescription
                })
                .ToListAsync(); 


            //var fgParentItems = await _tipsMasterDbContext.EnggBoms
            //    .Where(x => x.ItemNumber == saItemNumber && x.ItemType == PartType.FG)
            //    .Select(c => new EnggBomFGItemNumber()
            //    {
            //        ItemNumber = c.ItemNumber,
            //        Description = c.ItemDescription
            //    })
            //    .ToListAsync();

            return fgParentItems;
        }

        public async Task<IEnumerable<CoverageEnggChildDto>> GetEnggChildItemDetails(string ItemNumber)
        {
             

            return null;
        }

        public async Task<int> GetEnggBomId(string ItemNumber)
        {
            int enggBomId = _tipsMasterDbContext.EnggBoms
               .Where(e => e.ItemNumber == ItemNumber)
               .Select(e => e.BOMId)
               .FirstOrDefault();
            return enggBomId;
        }
        public async Task<IEnumerable<string>> GetEnggChildItemNumber(int enggBomId)
        {
            var enggBomIds = await _tipsMasterDbContext.EnggChildItems
               .Where(e => e.EnggBomId == enggBomId)
               .Select(e => e.ItemNumber)
                .ToListAsync();
            return enggBomIds;
        }
        public async Task<List<EnggChildItem>> GetEnggChildItemNumberByEnggbom(int bomId)
        {
            var enggBomIds = await _tipsMasterDbContext.EnggChildItems
               .Where(e => e.EnggBomId == bomId)
                .ToListAsync();
            return enggBomIds;
        }


        //public async Task<IEnumerable<EnggChildItem>> GeEnggBomChildByEnggBomId(int enggBomId)
        //{
        //    var enggBomIds = await _tipsMasterDbContext.EnggChildItems
        //       .Where(e => e.EnggBomId == enggBomId)
        //       .Include(x=>x.EnggAlternates)
        //        .ToListAsync();
        //    return enggBomIds;
        //}

        //end test2

        public async Task<IEnumerable<EnggBomFGItemNumber>> GetAllEnggBomChildFGItemNoListByItemNumber(string itemNumber)
        {

            // Replace this with the actual SA child item number you want to find the parent FG item number for

            var fgParentItemNumbers = await _tipsMasterDbContext.EnggChildItems
      .Where(x => x.ItemNumber == itemNumber && x.PartType == PartType.SA)
      .Join(
          _tipsMasterDbContext.EnggChildItems,
          childBomId => childBomId.EnggBomId,
          parentBomId => parentBomId.EnggBomId,
          (childBomId, parentBomId) => parentBomId
      )
      .Join(
          _tipsMasterDbContext.EnggBoms,
          parentItem => parentItem.EnggBomId,
          bom => bom.BOMId,
          (parentItem, bom) => bom
      )
      .Where(x => x.ItemType == PartType.FG)
      .Select(x => x.ItemNumber)
      .ToListAsync(); 
            return (IEnumerable<EnggBomFGItemNumber>)fgParentItemNumbers;
        }

        public async Task<EnggBom> GetEnggBomByFgPartNumber(string fgPartNumber)
        {
            var EnggBomDetailsbyId = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == fgPartNumber)                                             
                                .Include(t => t.EnggChildItems)
                                .ThenInclude(x => x.EnggAlternates)
                                .Include(m => m.NREConsumable)
                                             .FirstOrDefaultAsync();

            return EnggBomDetailsbyId;
        }
        //aravind
        public async Task<EnggBom> GetLatestEnggBomVersionDetailByItemNumber(string fgPartNumber, decimal revisionNo)
        {
            var EnggBomDetailsbyId = await _tipsMasterDbContext.EnggBoms
           .Where(x => x.ItemNumber == fgPartNumber && x.RevisionNumber == revisionNo)
            .Select(x => new
            {
                EnggBom = x,
                ActiveEnggChildItems = x.EnggChildItems.Where(ec => ec.IsActive)
            })
            .FirstOrDefaultAsync();

            EnggBomDetailsbyId.EnggBom.EnggChildItems = EnggBomDetailsbyId.ActiveEnggChildItems.ToList();

            return EnggBomDetailsbyId.EnggBom;

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
            enggBom.LastModifiedBy = _createdBy;
            enggBom.LastModifiedOn = DateTime.Now;
            Update(enggBom);
            string result = $"Engineering BOM Detail {enggBom.BOMId} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<object>> GetAllEnggBomItemNumberVersionList()
        {
            var enggBomDetails = _tipsMasterDbContext.EnggBoms
            .Where(x => x.IsEnggBomRelease == false)
            .GroupBy(bom => bom.ItemNumber)
            .Select(group => new
            {
                ItemNumber = group.Key,
                ItemDescription = group.Select(bom => bom.ItemDescription).FirstOrDefault(),
                RevisionNumbers = group.Select(bom => bom.RevisionNumber).ToArray()
            })
            .ToList();

            var enggBomItemNumberList = enggBomDetails
           .Select(bom => new EnggBomItemRevisionList
            {
                ItemNumber = bom.ItemNumber,
                RevisionNumber = bom.RevisionNumbers,
               ItemDescription = bom.ItemDescription
           }).ToList();

            return enggBomItemNumberList;
        }

        public async Task<EnggBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber,decimal revisionNumber)
        {
            var releaseEnggBom = await _tipsMasterDbContext.EnggBoms
            .Where(x => x.ItemNumber == itemNumber && x.RevisionNumber == revisionNumber)
            .FirstOrDefaultAsync();

            releaseEnggBom.IsEnggBomRelease= true;

            return releaseEnggBom;
        }

        //public async Task<IEnumerable<EngineeringBom>> GetAllEnggBomVersionListByItemNumber(string itemNumber)
        //{
        //    var enggBomDetails = await _tipsMasterDbContext.EngineeringBoms
        //   .Where(x => x.ItemNumber==itemNumber).ToListAsync();

        //    return enggBomDetails;
        //}

        public async Task<IEnumerable<EngineeringBom>> GetAllEnggBomVersionListByItemNumber(string itemNumber)
        { 
            var enggBomDetails = await _tipsMasterDbContext.EngineeringBoms
                .Where(x => x.ItemNumber == itemNumber)
                .ToListAsync();

            return enggBomDetails;
        }

        public async Task<EnggBom> GetEnggBomByItemNoAndRevNo(string itemNumber,decimal revisionNumber)
        {
            var EnggBomDetailsbyItemNumber = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == itemNumber && x.RevisionNumber == revisionNumber)
                               .Include(m => m.NREConsumable)
                               .Include(t => t.EnggChildItems)
                               .ThenInclude(x => x.EnggAlternates)
                             .FirstOrDefaultAsync();

            return EnggBomDetailsbyItemNumber;
        }
    }

    public class ReleaseEnggBomRepository : RepositoryBase<EngineeringBom>, IReleaseEnggBomRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext; 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReleaseEnggBomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext; 
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateReleaseEnggBom(EngineeringBom releaseEnggBom)
        {
            releaseEnggBom.CreatedBy = _createdBy;
            releaseEnggBom.CreatedOn = DateTime.Now;
            releaseEnggBom.LastModifiedBy = _createdBy;
            releaseEnggBom.LastModifiedOn = DateTime.Now;
            var result = await Create(releaseEnggBom);
            return result.Id;
        }

        public async Task<string> DeleteReleaseEnggBom(EngineeringBom releaseEnggBom)
        {
            Delete(releaseEnggBom);
            string result = $"ReleaseEnggBom details of {releaseEnggBom.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<EngineeringBom>> GetAllActiveReleaseEnggBom([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var enggBomDetails = FindAll()
                             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                                inv.ReleaseNote.Contains(searchParams.SearchValue) || inv.ReleaseFor.Contains(searchParams.SearchValue))));

            return PagedList<EngineeringBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<EngineeringBom>> GetAllReleaseEnggBom([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var enggBomDetails = FindAll().OrderByDescending(x => x.Id)
                          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                             inv.ReleaseNote.Contains(searchParams.SearchValue) || inv.ReleaseFor.Contains(searchParams.SearchValue))));

            return PagedList<EngineeringBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<EngineeringBom> GetReleaseEnggBomById(int id)
        {
            var ReleaseEnggBomDetailsbyId = await _tipsMasterDbContext.EngineeringBoms.Where(x => x.Id == id).FirstOrDefaultAsync();
            return ReleaseEnggBomDetailsbyId;
        }

        public async Task<string> UpdateReleaseEnggBom(EngineeringBom releaseEnggBom)
        {
            releaseEnggBom.LastModifiedBy = _createdBy;
            releaseEnggBom.LastModifiedOn = DateTime.Now;
            Update(releaseEnggBom);
            string result = $"ReleaseEnggBom Detail {releaseEnggBom.Id} is updated successfully!";
            return result;
        }
        public async Task<EngineeringBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber)
        {
            var releaseEnggBom = await _tipsMasterDbContext.EngineeringBoms
            .Where(x => x.ItemNumber == itemNumber && x.ReleaseVersion == revisionNumber)
            .FirstOrDefaultAsync();

            releaseEnggBom.IsReleaseCostCompleted = true;

            return releaseEnggBom;
        }
        public async Task<EngineeringBom> ReleasedEnggProductionByItemAndRevisionNumber(string itemNumber, decimal revisionNumber)
        {
            var releaseProductBom = await _tipsMasterDbContext.EngineeringBoms
            .Where(x => x.ItemNumber == itemNumber && x.ReleaseVersion == revisionNumber)
            .FirstOrDefaultAsync();

            releaseProductBom.IsReleaseProductCompleted = true;

            return releaseProductBom;
        }
    }

    public class ReleaseCostBomRepository : RepositoryBase<CostingBom>, IReleaseCostBomRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext; 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReleaseCostBomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateReleaseCostBom(CostingBom releaseCostBom)
        {
            releaseCostBom.CreatedBy = _createdBy;
            releaseCostBom.CreatedOn = DateTime.Now;
            releaseCostBom.LastModifiedBy = _createdBy;
            releaseCostBom.LastModifiedOn = DateTime.Now;
            var result = await Create(releaseCostBom);
            return result.Id;
        }

        public async Task<IEnumerable<object>> GetAllReleaseCostBomItemNumberVersionList()
        {
            var releaseCostBomDetails = _tipsMasterDbContext.EngineeringBoms
            .Where(x => x.IsReleaseCompleted == true && x.IsReleaseCostCompleted == false)
            .GroupBy(bom => bom.ItemNumber)
            .Select(group => new
            {
                ItemNumber = group.Key,
                ItemDescription = group.Select(bom=>bom.ItemDescription).FirstOrDefault(),
                RevisionNumbers = group.Select(bom => bom.ReleaseVersion).ToArray()
            })
            .ToList();

            var releaseCostBomItemNumberList = releaseCostBomDetails
           .Select(bom => new CostingBomItemRevisionList
           {
               ItemNumber = bom.ItemNumber,
               ItemDescription = bom.ItemDescription,
               ReleaseVersion = bom.RevisionNumbers
           }).ToList();

            return releaseCostBomItemNumberList;
        }
        public async Task<CostingBom> ReleasedCostBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber)
        {
            var releaseCostBom = await _tipsMasterDbContext.CostingBoms
            .Where(x => x.ItemNumber == itemNumber && x.ReleaseVersion == revisionNumber)
            .FirstOrDefaultAsync();

            releaseCostBom.IsReleaseProductCompleted = true;

            return releaseCostBom;
        }
        public async Task<PagedList<CostingBom>> GetAllCostingBom([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var costingBomDetails = FindAll().OrderByDescending(x => x.Id)
                           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                              inv.ReleaseNote.Contains(searchParams.SearchValue) || inv.ReleaseFor.Contains(searchParams.SearchValue))));

            return PagedList<CostingBom>.ToPagedList(costingBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<CostingBom> GetCostingBomById(int id)
        {
            var costingBomDetailsbyId = await _tipsMasterDbContext.CostingBoms.Where(x => x.Id == id)
                              .FirstOrDefaultAsync();

            return costingBomDetailsbyId;

        }

        public async Task<IEnumerable<CostingBom>> GetAllCostingBomVersionListByItemNumber(string itemNumber)
        {
            var costingBomDetails = await _tipsMasterDbContext.CostingBoms
             .Where(x => x.ItemNumber == itemNumber).ToListAsync();

            return costingBomDetails;
        }
    }

        public class ReleaseProductBomRepository : RepositoryBase<ProductionBom>, IReleaseProductBomRepository
        {
        private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReleaseProductBomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
            {
                _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateReleaseProductBom(ProductionBom releaseProductBom)
            {
                releaseProductBom.CreatedBy = _createdBy;
                releaseProductBom.CreatedOn = DateTime.Now;
                releaseProductBom.LastModifiedBy = _createdBy;
                releaseProductBom.LastModifiedOn = DateTime.Now;
                var result = await Create(releaseProductBom);
                return result.Id;
            }

            public async Task<IEnumerable<object>> GetAllReleaseProductBomItemNumberVersionList()
            {
                var releaseProductBomDetails = _tipsMasterDbContext.CostingBoms
                .Where(x => x.IsReleaseCostCompleted == true && x.IsReleaseProductCompleted == false)
                .GroupBy(bom => bom.ItemNumber)
                .Select(group => new
                {
                    ItemNumber = group.Key,
                    ItemDescription = group.Select(bom => bom.ItemDescription).FirstOrDefault(),
                    RevisionNumbers = group.Select(bom => bom.ReleaseVersion).ToArray()
                })
                .ToList();

                var releaseProductBomItemNumberList = releaseProductBomDetails
               .Select(bom => new GetAllReleaseProductBomItemNumberVersionList
               {
                   ItemNumber = bom.ItemNumber,
                   ItemDescription = bom.ItemDescription,
                   ReleaseVersion = bom.RevisionNumbers
               }).ToList();

                return releaseProductBomItemNumberList;
            }
        public async Task<PagedList<ProductionBom>> GetAllProductionBom([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var productionBomDetails = FindAll().OrderByDescending(x => x.Id)
                                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                                          inv.ReleaseNote.Contains(searchParams.SearchValue) || inv.ReleaseFor.Contains(searchParams.SearchValue))));

            return PagedList<ProductionBom>.ToPagedList(productionBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<ProductionBom> GetProductionBomById(int id)
        {
            var productionBomDetailsbyId = await _tipsMasterDbContext.ProductionBoms.Where(x => x.Id == id)
                                  .FirstOrDefaultAsync();

            return productionBomDetailsbyId;
        }

      
        public async Task<EnggBom> GetProductionBomByItemAndBomVersionNo(string itemNumber , decimal bomVersionNo)
        {
            var productionBomDetails = await _tipsMasterDbContext.EnggBoms
                                  .Where(x => x.ItemNumber == itemNumber && x.RevisionNumber == bomVersionNo && x.IsActive == true)
                                  .Include(x => x.EnggChildItems.Where(c=>c.IsActive == true))
                                  .Include(x => x.NREConsumable)
                                  .FirstOrDefaultAsync();

            return productionBomDetails;
        }

        public async Task<ProductionBom> GetProductionBomByItemNumber(string itemNumber, decimal bomRevisonNumber)
        {
            var productionBomDetail = await _tipsMasterDbContext.ProductionBoms
                                    .Where(x => x.ItemNumber == itemNumber && x.ReleaseVersion == bomRevisonNumber)
                                  .FirstOrDefaultAsync();

            return productionBomDetail;
        }

        public async Task<decimal> GetLatestProductionBomByItemNumber(string itemNumber)
        {

            decimal maxRevisionNumber = await _tipsMasterDbContext.ProductionBoms
    .Where(x => x.ItemNumber == itemNumber)
    .MaxAsync(x => x.ReleaseVersion);

            return maxRevisionNumber;
        }

        public async Task<IEnumerable<ProductionBom>> GetAllProductionBomVersionListByItemNumber(string itemNumber)
        {
            var productionBomDetails =await _tipsMasterDbContext.ProductionBoms
               .Where(x => x.ItemNumber == itemNumber)
             .ToListAsync();

            return productionBomDetails;
        }

        public async Task<IEnumerable<ProductionBomRevisionNumber>> GetAllProductionBomFGListByItemNumber(string itemNumber)
        {
            var releaseProductBomDetails = _tipsMasterDbContext.ProductionBoms
                 .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                 .Select(x => x.ReleaseVersion).ToArray();

            
                var releaseProductBomItemNumberList = releaseProductBomDetails
                   .Select(bom => new ProductionBomRevisionNumber
                   {
                       ItemNumber = itemNumber,
                       ItemType = PartType.FG,
                       BomVersionNo = releaseProductBomDetails
                   }).ToList();
                return releaseProductBomItemNumberList;
           
        }
      
        //aravind
        public async Task<IEnumerable<ProductionBomRevisionNumber>> GetAllProductionBomSAListByItemNumber(string itemNumber)
        {
            var releaseProductBomDetails = _tipsMasterDbContext.ProductionBoms
                 .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                 .Select(x => x.ReleaseVersion).ToArray();

            var enggChildItem = _tipsMasterDbContext.EnggChildItems
                .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                .Select(x => x.EnggBomId).Distinct().ToList();

            List<string> fgItemNumber = new List<string>();
            if (enggChildItem.Count > 0 && enggChildItem != null)
            {
                     fgItemNumber = _tipsMasterDbContext.EnggBoms
                    .Where(x => enggChildItem.Contains(x.BOMId) && x.ItemType == PartType.FG)
                    .Select(x => x.ItemNumber).Distinct().ToList();
            }

                var releaseProductBomItemNumberList = releaseProductBomDetails
                   .Select(bom => new ProductionBomRevisionNumber
                   {
                       ItemNumber = itemNumber,
                       FGItemNumber = fgItemNumber,
                       ItemType = PartType.SA,
                       BomVersionNo = releaseProductBomDetails
                   }).ToList();
            
                return releaseProductBomItemNumberList;
        }

    }

        public class EnggBomGroupRepository : RepositoryBase<EnggBomGroup>, IEnggBomGroupRepository
        {
        private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public EnggBomGroupRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
            {
                _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateEnggBomGroup(EnggBomGroup enggbomGroup)
            {
                enggbomGroup.CreatedBy = _createdBy;
                enggbomGroup.CreatedOn = DateTime.Now;
                enggbomGroup.LastModifiedBy = _createdBy;
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

        //public async Task<PagedList<EnggBomGroup>> GetAllActiveEnggBomGroup([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var enggBomGroupDetails = FindAll()
        //                     .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BomGroupName.Contains(searchParams.SearchValue) ||
        //                        inv.Remarks.Contains(searchParams.SearchValue))));

        //    return PagedList<EnggBomGroup>.ToPagedList(enggBomGroupDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<EnggBomGroup>> GetAllActiveEnggBomGroup()
        {
            var allActiveEnggBomGroup = await FindAll()
            .ToListAsync();
            return allActiveEnggBomGroup;
        }

        public async Task<IEnumerable<ListOfBomGroupDto>> GetAllBomGroupList()
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

        public async Task<PagedList<EnggBomGroup>> GetAllEnggBomGroup([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var enggBomGroupDetails = FindAll().OrderByDescending(x => x.Id)
                                     .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BomGroupName.Contains(searchParams.SearchValue) ||
                                        inv.Remarks.Contains(searchParams.SearchValue))));

            return PagedList<EnggBomGroup>.ToPagedList(enggBomGroupDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        //public async Task<IEnumerable<EnggBomGroup>> GetAllEnggBomGroup()
        //{
        //    var enggBomGroupDetails = FindAll().OrderByDescending(x => x.Id);
        //    return enggBomGroupDetails;
        //}
        public async Task<EnggBomGroup> GetEnggBomGroupById(int id)
            {
                var EnggbomGroupDetailsbyId = await _tipsMasterDbContext.BomGroups.Where(x => x.Id == id).FirstOrDefaultAsync();
                return EnggbomGroupDetailsbyId;
            }

            public async Task<string> UpdateEnggBomGroup(EnggBomGroup enggbomGroup)
            {
                enggbomGroup.LastModifiedBy = _createdBy;
                enggbomGroup.LastModifiedOn = DateTime.Now;
                Update(enggbomGroup);
                string result = $"EnggBomGroup Detail {enggbomGroup.Id} is updated successfully!";
                return result;
            }
        }
        public class EnggCustomFieldRepository : RepositoryBase<EnggCustomField>, IEnggCustomFieldRepository
        {
            private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public EnggCustomFieldRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
            {
                _tipsMasterDbContext = repositoryContext; 
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateEnggCustomField(EnggCustomField enggcustomFields)
            {
                enggcustomFields.CreatedBy = _createdBy;
                enggcustomFields.CreatedOn = DateTime.Now;
                enggcustomFields.LastModifiedBy = _createdBy;
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

        //public async Task<PagedList<EnggCustomField>> GetAllActiveEnggCustomFields([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var enggCustomFieldDetails = FindAll()
        //.Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BOMGroupName.Contains(searchParams.SearchValue) ||
        //inv.LabelName.Contains(searchParams.SearchValue))));

        //    return PagedList<EnggCustomField>.ToPagedList(enggCustomFieldDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<EnggCustomField>> GetAllActiveEnggCustomFields()
        {
            var allActiveEnggCustomField = await FindAll()
            .ToListAsync();
            return allActiveEnggCustomField;
        }

        //public async Task<PagedList<EnggCustomField>> GetAllEnggCustomFields([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var enggCustomFieldDetails = FindAll().OrderByDescending(x => x.Id)
        //                 .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BOMGroupName.Contains(searchParams.SearchValue) ||
        //                    inv.LabelName.Contains(searchParams.SearchValue))));

        //    return PagedList<EnggCustomField>.ToPagedList(enggCustomFieldDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
        public async Task<IEnumerable<EnggCustomField>> GetAllEnggCustomFields()
        {
            var enggCustomFields = FindAll().OrderByDescending(x => x.Id);
            return enggCustomFields;
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
                enggcustomFields.LastModifiedBy = _createdBy;
                enggcustomFields.LastModifiedOn = DateTime.Now;
                Update(enggcustomFields);
                string result = $"EnggCustomFields Detail {enggcustomFields.Id} is updated successfully!";
                return result;
            }
        }

        public class EngineeringNREConsumableRepository : RepositoryBase<NREConsumable>, IEnggBomNREConsumableRepository
        {
            private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public EngineeringNREConsumableRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
            {
                _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

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
