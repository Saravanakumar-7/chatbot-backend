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
        public async Task<FGFinalLandedandMoqPrice> GetEngganditsPP(string FGItemNumber, decimal FGRevno, List<RfqSourcingPPdetailsforEngg> rfqSourcingPPdetails)
        {
            var FGid = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == FGItemNumber && x.RevisionNumber == FGRevno && x.IsActive == true).Select(x => x.BOMId).FirstOrDefaultAsync();
            List<EnggChildItem> FGchilditems = await _tipsMasterDbContext.EnggChildItems.Where(x => x.EnggBomId == FGid && x.IsActive == true).ToListAsync();
            FGFinalLandedandMoqPrice FGFinalLandedandMoqPrice = new FGFinalLandedandMoqPrice()
            {
                FGItemNumber = FGItemNumber,
                FinalLandindPrice = 0,
                FinalMoqcost = 0
            };
            SAFinalLandedandMoqPrice sAFinalLandedandMoqPrice = new SAFinalLandedandMoqPrice()
            {
                SAFinalLandindPrice = 0,
                SAFinalMoqcost = 0
            };
            SAFinalLandedandMoqPrice ChildsAFinalLandedandMoqPrice = new SAFinalLandedandMoqPrice()
            {
                SAFinalLandindPrice = 0,
                SAFinalMoqcost = 0
            };
            decimal? FGfsum = 0;
            decimal? FGmsum = 0;
            foreach (var fgitem in FGchilditems)
            {
                if (fgitem.PartType == PartType.SA)
                {
                    ChildsAFinalLandedandMoqPrice = await GetEnggFGSA(fgitem.ItemNumber, fgitem.Quantity, rfqSourcingPPdetails);
                    sAFinalLandedandMoqPrice.SAFinalLandindPrice = sAFinalLandedandMoqPrice.SAFinalLandindPrice + ChildsAFinalLandedandMoqPrice.SAFinalLandindPrice;
                    sAFinalLandedandMoqPrice.SAFinalMoqcost = sAFinalLandedandMoqPrice.SAFinalMoqcost + ChildsAFinalLandedandMoqPrice.SAFinalMoqcost;
                }
                if (fgitem.PartType == PartType.PurchasePart)
                {
                    foreach (var FPPdetails in rfqSourcingPPdetails)
                    {
                        if (fgitem.ItemNumber == FPPdetails.PPItemNumber)
                        {
                            if (FPPdetails.VLandindPrice == null)
                            {
                                FPPdetails.VLandindPrice = 1;
                            }
                            if (FPPdetails.VMoqcost == null)
                            {
                                FPPdetails.VMoqcost = 1;
                            }
                            decimal? landedprice = fgitem.Quantity * FPPdetails.VLandindPrice;
                            decimal? moqcost = fgitem.Quantity * FPPdetails.VMoqcost;
                            FGfsum = FGfsum + landedprice;
                            FGmsum = FGmsum + moqcost;
                        }
                    }
                }
            }
            FGFinalLandedandMoqPrice.FinalLandindPrice = FGfsum + sAFinalLandedandMoqPrice.SAFinalLandindPrice;
            FGFinalLandedandMoqPrice.FinalMoqcost = FGmsum + sAFinalLandedandMoqPrice.SAFinalMoqcost;
            return FGFinalLandedandMoqPrice;
        }
        public async Task<SAFinalLandedandMoqPrice> GetEnggFGSA(string SAItemNumber, decimal SAQty, List<RfqSourcingPPdetailsforEngg> rfqSourcingPPdetails)
        {
            var SAid = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == SAItemNumber && x.IsActive == true).OrderByDescending(x => x.BOMId).Select(x => x.BOMId).FirstOrDefaultAsync();
            List<EnggChildItem> SAchilditems = await _tipsMasterDbContext.EnggChildItems.Where(x => x.EnggBomId == SAid && x.IsActive == true).ToListAsync();
            SAFinalLandedandMoqPrice sAFinalLandedandMoqPrice = new SAFinalLandedandMoqPrice()
            {
                SAFinalLandindPrice = 0,
                SAFinalMoqcost = 0
            };
            SAFinalLandedandMoqPrice ChildsAFinalLandedandMoqPrice = new SAFinalLandedandMoqPrice()
            {
                SAFinalLandindPrice = 0,
                SAFinalMoqcost = 0
            };
            decimal? SAfsum = 0;
            decimal? SAmsum = 0;
            foreach (var saitem in SAchilditems)
            {
                if (saitem.PartType == PartType.SA)
                {
                    SAFinalLandedandMoqPrice TotalChildsAFinalLandedandMoqPrice = new SAFinalLandedandMoqPrice();
                    TotalChildsAFinalLandedandMoqPrice = await GetEnggFGSA(saitem.ItemNumber, saitem.Quantity, rfqSourcingPPdetails);
                    ChildsAFinalLandedandMoqPrice.SAFinalLandindPrice = TotalChildsAFinalLandedandMoqPrice.SAFinalLandindPrice + ChildsAFinalLandedandMoqPrice.SAFinalLandindPrice;
                    ChildsAFinalLandedandMoqPrice.SAFinalMoqcost = TotalChildsAFinalLandedandMoqPrice.SAFinalMoqcost + ChildsAFinalLandedandMoqPrice.SAFinalMoqcost;
                }
                if (saitem.PartType == PartType.PurchasePart)
                {
                    foreach (var SPPdetails in rfqSourcingPPdetails)
                    {
                        if (saitem.ItemNumber == SPPdetails.PPItemNumber)
                        {
                            if (SPPdetails.VLandindPrice == null)
                            {
                                SPPdetails.VLandindPrice = 1;
                            }
                            if (SPPdetails.VMoqcost == null)
                            {
                                SPPdetails.VMoqcost = 1;
                            }
                            decimal? landedprice = saitem.Quantity * SPPdetails.VLandindPrice;
                            decimal? moqcost = saitem.Quantity * SPPdetails.VMoqcost;
                            SAfsum = SAfsum + landedprice;
                            SAmsum = SAmsum + moqcost;
                        }
                    }
                }
            }
            sAFinalLandedandMoqPrice.SAFinalLandindPrice = (SAfsum + ChildsAFinalLandedandMoqPrice.SAFinalLandindPrice) * SAQty;
            sAFinalLandedandMoqPrice.SAFinalMoqcost = (SAmsum + ChildsAFinalLandedandMoqPrice.SAFinalMoqcost) * SAQty;
            return sAFinalLandedandMoqPrice;
        }
        public async Task<EnggBom> UpdateEnggBomVersion(EnggBom enggBom)
        {
            enggBom.CreatedBy = _createdBy;
            enggBom.CreatedOn = DateTime.Now;
            enggBom.Unit = _unitname;
            var getOldRevisionNumber = _tipsMasterDbContext.EnggBoms
                .Where(x => x.ItemNumber == enggBom.ItemNumber)
                .OrderByDescending(x => x.BOMId)
                .FirstOrDefault();
            getOldRevisionNumber.LastModifiedBy = _createdBy;
            getOldRevisionNumber.LastModifiedOn = DateTime.Now;
            Update(getOldRevisionNumber);
            enggBom.RevisionNumber = getOldRevisionNumber.RevisionNumber;
            var result = await Create(enggBom);
            return result;

        }
        public async Task<IEnumerable<EnggBomSPReport>> GetEnggBomSPReportWithParam(int? bomId)
        {
            var result = _tipsMasterDbContext
            .Set<EnggBomSPReport>()
            .FromSqlInterpolated($"CALL enggbom_report_parameter({bomId})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<EnggBomSPReport>> GetEnggBomRevSPReportWithParam(string itemNumber, decimal revisionNumber)
        {
            var result = _tipsMasterDbContext
            .Set<EnggBomSPReport>()
            .FromSqlInterpolated($"CALL enggbom_report_revision({itemNumber},{revisionNumber})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<FGCostingSPReport>> GetFGCostingSPReportWithParam(string fgItemnumber, string shopOrderNumber)
        {
            var result = _tipsMasterDbContext
            .Set<FGCostingSPReport>()
            .FromSqlInterpolated($"CALL FG_Costing_Report_with_parameter({fgItemnumber},{shopOrderNumber})")
            .ToList();

            return result;

        }
        //public async Task<BomSPReport> GetBomDetailsSPReportWithParam(string itemNumber)
        //{
        //    var result = _tipsMasterDbContext
        //    .Set<BomSPReport>()
        //    .FromSqlInterpolated($"CALL Bom_GetBy_ItemNumber({itemNumber})")
        //    .ToList();

        //    return result[0];

        //} 
        public async Task<List<EnggBomLevelSPReport>> GetEnggLevelsSPReport(string itemNumber)
        {
            var result = _tipsMasterDbContext
            .Set<EnggBomLevelSPReport>()
            .FromSqlInterpolated($"CALL Bom_GetBy_ItemNumber({itemNumber})")
            .ToList();

            return result;

        }

        public async Task<int?> CreateEnggBom(EnggBom enggBom)
        {
            enggBom.CreatedBy = _createdBy;
            enggBom.CreatedOn = DateTime.Now;
            // enggBom.LastModifiedBy = _createdBy;
            //  enggBom.LastModifiedOn = DateTime.Now;
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

        //get latest revisionno and isbomrelease
        public async Task<EnggBom> GetAllLatestRevAndIsReleaseEnggBom(string itemNumber)
        {
            var getalllatestReleaseEnggBom = await FindByCondition(x => x.IsActive == true && x.IsEnggBomRelease == true)
             .OrderByDescending(bom => bom.RevisionNumber)
             .Where(x => x.ItemNumber == itemNumber)
            .Include(t => t.EnggChildItems)
            .ThenInclude(t => t.EnggAlternates)
            .Include(t => t.NREConsumable)
            .FirstOrDefaultAsync();
            return getalllatestReleaseEnggBom;
        }
        public async Task<EnggBom> GetAllLatestRevBOMIsReleaseEnggBom(string itemNumber)
        {
            var latestBomData = await FindByCondition(x => x.IsActive == true && x.IsEnggBomRelease == true)
             .OrderByDescending(bom => bom.RevisionNumber)
             .Where(x => x.ItemNumber == itemNumber)
            .FirstOrDefaultAsync();
            return latestBomData;
        }
        public async Task<List<EnggChildItem>> GetChildItemsLists()
        {
            var list = await _tipsMasterDbContext.EnggChildItems.ToListAsync();
            return list;
        }

        public async Task<PagedList<EnggBom>> GetAllEnggBOM([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            PartType? check;
            if (Enum.TryParse<PartType>(searchParams.SearchValue, out PartType result))
            {
                check = result;
            }
            else
            {
                check = null;
            }
            int searchValueAsInt;
            bool isSearchValueNumeric = int.TryParse(searchParams.SearchValue, out searchValueAsInt);
            var enggBomDetails = FindAll().OrderByDescending(x => x.BOMId)
                          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                          || inv.ItemNumber.Contains(searchParams.SearchValue)
                          || inv.ItemDescription.Contains(searchParams.SearchValue)
                          || (isSearchValueNumeric && inv.RevisionNumber.Equals(searchValueAsInt))
                          || inv.ItemType.Equals(check))))
                                .Include(t => t.EnggChildItems)
                               .ThenInclude(t => t.EnggAlternates)
                               .Include(t => t.NREConsumable);

            return PagedList<EnggBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        //public async Task<PagedList<EnggBom>> GetAllEnggBOM([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{

        //    var enggBomDetails = FindAll().OrderByDescending(x => x.BOMId)
        //                  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
        //                     /*inv.ItemType.Equals(int.Parse(searchParams.SearchValue)) ||*/ inv.ItemDescription.Contains(searchParams.SearchValue))))
        //                        .Include(t => t.EnggChildItems)
        //                       .ThenInclude(t => t.EnggAlternates)
        //                       .Include(t => t.NREConsumable);

        //    return PagedList<EnggBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
        public async Task<IEnumerable<EnggBomItemDto>> GetAllEnggBOMItemNumber()
        {
            IEnumerable<EnggBomItemDto> getAllEnggBomItems = await _tipsMasterDbContext.EnggBoms
                .Where(x => x.IsActive == true)
            .Select(c => new EnggBomItemDto()
            {
                ItemNumber = c.ItemNumber,
            })
           .ToListAsync();
            return getAllEnggBomItems;

        }

        //public async Task<IEnumerable<FGItemNumberListDto>> GetAllEnggBOMDetailsByItemNumber(string fgItemNumber)
        //{
        //    var enggBomItemDetails = await _tipsMasterDbContext.EnggChildItems
        //                            .Where(x => _tipsMasterDbContext.EnggBoms
        //                                .Where(bom => bom.ItemNumber == fgItemNumber && bom.ItemType == PartType.FG)
        //                                .Select(bom => bom.BOMId)
        //                                .Contains(x.EnggBomId))
        //                                .GroupBy(x => new { x.ItemNumber })
        //                                .Select(group => new FGItemNumberListDto
        //                                 {
        //                                     BomVersionNo = _tipsMasterDbContext.EnggBoms
        //                                        .Where(bom => group.Select(g => g.EnggBomId).Contains(bom.BOMId))
        //                                        .Max(bom => bom.RevisionNumber),
        //                                         Qty = group.Sum(x => x.Quantity)
        //                                })
        //                                .ToListAsync();

        //    return enggBomItemDetails;

        //}

        //public async Task<List<EnggBomFGItemNumberWithQtyDto>> GetFGBomItemsChildDetails(List<string> itemNumberList)
        //{
        //    var itemIdNoList = await TipsMasterDbContext.EnggBoms
        //        .Where(im => itemNumberList.Contains(im.ItemNumber) && im.IsEnggBomRelease == true)
        //        .Select(x => new { x.ItemNumber, x.BOMId, x.ItemType }).Distinct().ToListAsync();

        //    List<int> itemNos = itemIdNoList.Where(x => x.ItemType == PartType.FG).Select(x => x.BOMId).Distinct().ToList();

        //    List<EnggBomFGItemNumberWithQtyDto> result = _tipsMasterDbContext.EnggChildItems
        //            .Where(x => itemNos.Contains(x.EnggBomId))
        //            .GroupBy(l => new { l.ItemNumber, l.Description })
        //            .Select(group => new EnggBomFGItemNumberWithQtyDto
        //            {
        //                ItemNumber = group.Key.ItemNumber,
        //                QtyReq = group.Sum(c => c.Quantity),
        //                ItemDescription = group.Key.Description
        //            }).ToList();

        //    return result;
        //}
        //sa

        public async Task<List<EnggBomFGItemNumberWithQtyDto>> GetFGBomItemsChildDetails(List<RfqEnggitemSourcingDto> itemNumberList)
        {
            List<EnggBomFGItemNumberWithQtyDto> enggBomFGItemNumberWithQtyDtos = new List<EnggBomFGItemNumberWithQtyDto>();
            foreach (var rfqfgitem in itemNumberList)
            {
                int fgdetails = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == rfqfgitem.ItemNumber && x.RevisionNumber == rfqfgitem.CostingBomVersionNo && x.IsActive == true)
                    .Select(x => x.BOMId)
                    .FirstOrDefaultAsync();
                var fgbomdetails = await _tipsMasterDbContext.EnggChildItems.Where(x => x.EnggBomId == fgdetails && x.IsActive == true).OrderByDescending(x => x.PartType).ToListAsync();
                if (fgbomdetails != null)
                {
                    foreach (var childofFG in fgbomdetails)
                    {
                        if (childofFG.PartType == PartType.PurchasePart)
                        {
                            int flag = 0;
                            foreach (var existingPP in enggBomFGItemNumberWithQtyDtos)
                            {
                                if (existingPP.ItemNumber == childofFG.ItemNumber)
                                {
                                    existingPP.QtyReq = existingPP.QtyReq + (childofFG.Quantity * rfqfgitem.Qty);
                                    flag = 1;
                                    break;
                                }
                            }
                            if (flag == 0)
                            {
                                EnggBomFGItemNumberWithQtyDto addPP = new EnggBomFGItemNumberWithQtyDto();
                                addPP.ItemNumber = childofFG.ItemNumber;
                                addPP.ItemDescription = childofFG.Description;
                                addPP.QtyReq = childofFG.Quantity * rfqfgitem.Qty;
                                enggBomFGItemNumberWithQtyDtos.Add(addPP);
                            }
                        }
                        if (childofFG.PartType == PartType.SA)
                        {
                            var subSA = await GetSABomItemsChildDetails(childofFG.ItemNumber, childofFG.Quantity);//, childofFG.Version);
                            foreach (var existingpp in subSA)
                            {
                                int flag = 0;
                                foreach (var existingPP in enggBomFGItemNumberWithQtyDtos)
                                {
                                    if (existingpp.ItemNumber == existingPP.ItemNumber)
                                    {
                                        existingPP.QtyReq = existingPP.QtyReq + (existingpp.QtyReq * rfqfgitem.Qty);
                                        flag = 1;
                                        break;
                                    }
                                }
                                if (flag == 0)
                                {
                                    EnggBomFGItemNumberWithQtyDto addPP = new EnggBomFGItemNumberWithQtyDto();
                                    addPP.ItemNumber = existingpp.ItemNumber;
                                    addPP.ItemDescription = existingpp.ItemDescription;
                                    addPP.QtyReq = existingpp.QtyReq * rfqfgitem.Qty;
                                    enggBomFGItemNumberWithQtyDtos.Add(addPP);
                                }
                            }
                        }
                    }
                }
            }
            return enggBomFGItemNumberWithQtyDtos;
        }
        public async Task<List<EnggBomFGCostItemNumberWithQtyDto>> GetFGBomItemsChildCostingDetails(string fgItemMaster)
        {
            List<EnggBomFGCostItemNumberWithQtyDto> enggBomFGItemNumberWithQtyDtos = new List<EnggBomFGCostItemNumberWithQtyDto>();

            int bomId = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == fgItemMaster && x.IsEnggBomRelease == true && x.IsActive == true)
                .OrderByDescending(x => x.RevisionNumber)
                .Select(x => x.BOMId)
                .FirstOrDefaultAsync();

            var fgbomdetails = await _tipsMasterDbContext.EnggChildItems.Where(x => x.EnggBomId == bomId && x.IsActive == true).OrderByDescending(x => x.PartType).ToListAsync();
            if (fgbomdetails != null)
            {
                foreach (var childofFG in fgbomdetails)
                {
                    if (childofFG.PartType == PartType.PurchasePart)
                    {
                        int flag = 0;
                        foreach (var existingPP in enggBomFGItemNumberWithQtyDtos)
                        {
                            if (existingPP.ItemNumber == childofFG.ItemNumber)
                            {
                                existingPP.QtyReq = existingPP.QtyReq + (childofFG.Quantity);
                                flag = 1;
                                break;
                            }
                        }
                        if (flag == 0)
                        {
                            EnggBomFGCostItemNumberWithQtyDto addPP = new EnggBomFGCostItemNumberWithQtyDto();
                            addPP.FGItemNumber = fgItemMaster;
                            addPP.ItemNumber = childofFG.ItemNumber;
                            addPP.ItemDescription = childofFG.Description;
                            addPP.QtyReq = childofFG.Quantity;
                            enggBomFGItemNumberWithQtyDtos.Add(addPP);
                        }
                    }
                    if (childofFG.PartType == PartType.SA)
                    {
                        var subSA = await GetSABomItemsChildDetails(childofFG.ItemNumber, childofFG.Quantity);
                        foreach (var existingpp in subSA)
                        {
                            int flag = 0;
                            foreach (var existingPP in enggBomFGItemNumberWithQtyDtos)
                            {
                                if (existingpp.ItemNumber == existingPP.ItemNumber)
                                {
                                    existingPP.QtyReq = existingPP.QtyReq + (existingpp.QtyReq);
                                    flag = 1;
                                    break;
                                }
                            }
                            if (flag == 0)
                            {
                                EnggBomFGCostItemNumberWithQtyDto addPP = new EnggBomFGCostItemNumberWithQtyDto();
                                addPP.FGItemNumber = fgItemMaster;
                                addPP.ItemNumber = existingpp.ItemNumber;
                                addPP.ItemDescription = existingpp.ItemDescription;
                                addPP.QtyReq = existingpp.QtyReq;
                                enggBomFGItemNumberWithQtyDtos.Add(addPP);
                            }
                        }
                    }
                }
            }

            return enggBomFGItemNumberWithQtyDtos;
        }
        public async Task<List<EnggBomFGItemNumberWithQtyDto>> GetSABomItemsChildDetails(string SAitemnumber, decimal SAQty)//, string SAversion)
        {
            List<EnggBomFGItemNumberWithQtyDto> enggBomSAItemNumberWithQtyDtos = new List<EnggBomFGItemNumberWithQtyDto>();
            int sadetails = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == SAitemnumber && x.IsActive == true).OrderByDescending(x => x.RevisionNumber)
                //&& x.RevisionNumber == decimal.Parse(SAversion))
                .Select(x => x.BOMId).FirstOrDefaultAsync();
            var sabomdetails = await _tipsMasterDbContext.EnggChildItems.Where(x => x.EnggBomId == sadetails && x.IsActive == true).OrderByDescending(x => x.PartType).ToListAsync();
            if (sabomdetails != null)
            {
                foreach (var childofSA in sabomdetails)
                {
                    if (childofSA.PartType == PartType.PurchasePart)
                    {
                        int flag = 0;
                        foreach (var existingPP in enggBomSAItemNumberWithQtyDtos)
                        {
                            if (existingPP.ItemNumber == childofSA.ItemNumber)
                            {
                                existingPP.QtyReq = existingPP.QtyReq + (childofSA.Quantity * SAQty);
                                flag = 1;
                                break;
                            }
                        }
                        if (flag == 0)
                        {
                            EnggBomFGItemNumberWithQtyDto addPP = new EnggBomFGItemNumberWithQtyDto();
                            addPP.ItemNumber = childofSA.ItemNumber;
                            addPP.ItemDescription = childofSA.Description;
                            addPP.QtyReq = childofSA.Quantity * SAQty;
                            enggBomSAItemNumberWithQtyDtos.Add(addPP);
                        }
                    }
                    if (childofSA.PartType == PartType.SA)
                    {
                        var subSA = await GetSABomItemsChildDetails(childofSA.ItemNumber, childofSA.Quantity);//, childofSA.Version);
                        foreach (var existingpp in subSA)
                        {
                            int flag = 0;
                            foreach (var existingPP in enggBomSAItemNumberWithQtyDtos)
                            {
                                if (existingpp.ItemNumber == existingPP.ItemNumber)
                                {
                                    existingPP.QtyReq = existingPP.QtyReq + (existingpp.QtyReq * SAQty);
                                    flag = 1;
                                    break;
                                }
                            }
                            if (flag == 0)
                            {
                                EnggBomFGItemNumberWithQtyDto addPP = new EnggBomFGItemNumberWithQtyDto();
                                addPP.ItemNumber = existingpp.ItemNumber;
                                addPP.ItemDescription = existingpp.ItemDescription;
                                addPP.QtyReq = existingpp.QtyReq * SAQty;
                                enggBomSAItemNumberWithQtyDtos.Add(addPP);
                            }
                        }
                    }
                }
            }
            return enggBomSAItemNumberWithQtyDtos;
        }
        public async Task<decimal?> GetSABomQuantity(string fgPartNumber, string saItemNumber)
        {

            var maxRevisionNumber = await _tipsMasterDbContext.EnggBoms
                    .Where(x => x.ItemNumber == fgPartNumber && x.ItemType == PartType.FG && x.IsActive == true)
                    .MaxAsync(x => x.RevisionNumber);

            var bomId = await _tipsMasterDbContext.EnggBoms
                .Where(x => x.ItemNumber == fgPartNumber && x.ItemType == PartType.FG
                        && x.RevisionNumber == maxRevisionNumber && x.IsActive == true)
                .Select(x => x.BOMId)
                .FirstOrDefaultAsync();


            var sumOfQuantity = await _tipsMasterDbContext.EnggChildItems
                 .Where(x => x.EnggBomId == bomId && x.ItemNumber == saItemNumber && x.IsActive == true)
                 .SumAsync(x => x.Quantity);

            return sumOfQuantity;
        }

        public async Task<IEnumerable<EnggBomFGItemNumber>> GetAllEnggBomFGItemNoListByItemNumber(string itemNumber)
        {
            List<int> bomDetails = await _tipsMasterDbContext.EnggChildItems
                                .Where(x => x.ItemNumber == itemNumber && x.IsActive == true && (x.PartType == PartType.SA || x.PartType == PartType.PurchasePart))
                                .Select(x => x.EnggBomId).Distinct().ToListAsync();

            IEnumerable<EnggBomFGItemNumber> getAllBomGroupList = await _tipsMasterDbContext.EnggBoms
                .Where(x => bomDetails.Contains(x.BOMId) && x.IsActive == true)
                .Select(c => new EnggBomFGItemNumber()
                {
                    ItemNumber = c.ItemNumber,
                    Description = c.ItemDescription
                })
                             .ToListAsync();

            return getAllBomGroupList;
        }

        public async Task<IEnumerable<EnggBomDetailsDto>> GetAllEnggBomDetailsByItemNumber(string itemNumber)
        {

            IEnumerable<EnggBomDetailsDto> getAllBomGroupList = await _tipsMasterDbContext.EnggBoms
                .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                .Select(c => new EnggBomDetailsDto()
                {
                    ItemNumber = c.ItemNumber,
                    ItemDescription = c.ItemDescription,
                    ItemType = c.ItemType,
                    IsActive = c.IsActive
                })
              .ToListAsync();

            return getAllBomGroupList;
        }
        public async Task<bool> CheckEnggBomByItemNumber(string itemNumber)
        {

            var getAllBomGroupList = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == itemNumber).CountAsync();

            return getAllBomGroupList>0;
        }

        public async Task<IEnumerable<EnggChildBomDetailsDto>> GetAllEnggChildBomDetailsByItemNumber(string itemNumber)
        {

            IEnumerable<EnggChildBomDetailsDto> getAllBomGroupList = await _tipsMasterDbContext.EnggChildItems
                .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                .Select(c => new EnggChildBomDetailsDto()
                {
                    ItemNumber = c.ItemNumber,
                    MftrItemNumbers = c.MftrItemNumbers,
                    UOM = c.UOM,
                    Description = c.Description,
                    PartType = c.PartType,
                    IsActive = c.IsActive
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
                .Where(x => x.ItemNumber == saItemNumber && x.IsActive == true)
                .Select(x => x.EnggBomId)
                .FirstOrDefaultAsync();

            //if (saEnggBomId == null)
            //{
            //    // SA item number not found, return null or handle the situation accordingly
            //    return null;
            //}

            // Fetch the parent FG item number using the EnggBomId
            var parentFgItemNumber = await _tipsMasterDbContext.EnggBoms
                .Where(x => x.BOMId == saEnggBomId && x.ItemType == PartType.FG && x.IsActive == true)
                .Select(x => x.ItemNumber)
                .FirstOrDefaultAsync();

            //if (parentFgItemNumber == null)
            //{
            //    // Parent FG item number not found, return null or handle the situation accordingly
            //    return null;
            //}

            // Check if the parent FG item has any child SA items and recursively find their parent FG item numbers
            var childSaItemNumbers = await _tipsMasterDbContext.EnggChildItems
                .Where(x => x.EnggBomId == saEnggBomId && x.PartType == PartType.SA && x.IsActive == true)
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

        public async Task<List<EnggBomFGItemNumber>> GetAllFgItemNumberListBySaItemNumber(string saItemNumber)
        {
            List<EnggBomFGItemNumber> fgParents = new List<EnggBomFGItemNumber>();

            // Identify all the parent BOMs by checking which BOMs have this current item in their EnggChildItems.
            var parentBoms = await _tipsMasterDbContext.EnggBoms
                .Include(b => b.EnggChildItems)
                .Where(bom => bom.EnggChildItems.Any(child => child.ItemNumber == saItemNumber) && bom.IsActive == true)
                .ToListAsync();

            foreach (var parentBom in parentBoms)
            {
                // If the parent BOM's ItemType is FG, add it to the list.
                if (parentBom.ItemType == PartType.FG)
                {
                    EnggBomFGItemNumber enggBomFGItemNumber = new EnggBomFGItemNumber
                    {
                        ItemNumber = parentBom.ItemNumber,
                        Description = parentBom.ItemDescription
                    };
                    fgParents.Add(enggBomFGItemNumber);
                }

                // If the parent BOM's ItemType is SA, recursively search for its FG parents and add them to the list.
                else if (parentBom.ItemType == PartType.SA)
                {
                    var fgParentsFromRecursion = await GetAllFgItemNumberListBySaItemNumber(parentBom.ItemNumber);
                    fgParents.AddRange(fgParentsFromRecursion);
                }
            }

            return fgParents;
        }


        public async Task<IEnumerable<CoverageEnggChildDto>> GetEnggChildItemDetails(string ItemNumber)
        {


            return null;
        }

        public async Task<int> GetEnggBomId(string ItemNumber)
        {
            int enggBomId = _tipsMasterDbContext.EnggBoms
               .Where(e => e.ItemNumber == ItemNumber && e.IsActive == true)
               .Select(e => e.BOMId)
               .FirstOrDefault();
            return enggBomId;
        }
        public async Task<IEnumerable<string>> GetEnggChildItemNumber(int enggBomId)
        {
            var enggBomIds = await _tipsMasterDbContext.EnggChildItems
               .Where(e => e.EnggBomId == enggBomId && e.IsActive == true)
               .Select(e => e.ItemNumber)
                .ToListAsync();
            return enggBomIds;
        }
        public async Task<List<EnggChildItem>> GetEnggChildItemNumberByEnggbom(int bomId)
        {
            var enggBomIds = await _tipsMasterDbContext.EnggChildItems
               .Where(e => e.EnggBomId == bomId && e.IsActive == true)
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
      .Where(x => x.ItemNumber == itemNumber && x.PartType == PartType.SA && x.IsActive == true)
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
            var EnggBomDetailsbyId = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == fgPartNumber && x.IsActive == true)
                                .Include(t => t.EnggChildItems)
                                .ThenInclude(x => x.EnggAlternates)
                                .Include(m => m.NREConsumable)
                                             .FirstOrDefaultAsync();

            return EnggBomDetailsbyId;
        }
        //aravind
        public async Task<EnggBom> GetLatestEnggBomVersionDetailByItemNumber(string fgPartNumber, decimal revisionNo)
        {
            //var EnggBomDetailsbyId = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == fgPartNumber 
            //                        && x.RevisionNumber == revisionNo)
            //                        .Include(t => t.EnggChildItems)
            //                        .FirstOrDefaultAsync();

            //return EnggBomDetailsbyId;

            var EnggBomDetailsbyId = await _tipsMasterDbContext.EnggBoms
           .Where(x => x.ItemNumber == fgPartNumber && x.RevisionNumber == revisionNo && x.IsActive == true)
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
            .Where(x => x.IsEnggBomRelease == false && x.IsActive == true)
            .GroupBy(bom => bom.ItemNumber)
            .Select(group => new
            {
                ItemNumber = group.Key,
                ItemDescription = group.Select(bom => bom.ItemDescription).FirstOrDefault(),
                RevisionNumbers = group.Select(bom => bom.RevisionNumber).ToArray(),
                ItemType = group.Select(bom => bom.ItemType).FirstOrDefault()
            })
            .ToList();

            var enggBomItemNumberList = enggBomDetails
           .Select(bom => new EnggBomItemRevisionList
           {
               ItemNumber = bom.ItemNumber,
               RevisionNumber = bom.RevisionNumbers,
               ItemDescription = bom.ItemDescription,
               ItemType = bom.ItemType
           }).ToList();

            return enggBomItemNumberList;
        }

        public async Task<EnggBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber)
        {
            var releaseEnggBom = await _tipsMasterDbContext.EnggBoms
            .Where(x => x.ItemNumber == itemNumber && x.RevisionNumber == revisionNumber && x.IsActive == true)
            .FirstOrDefaultAsync();

            releaseEnggBom.IsEnggBomRelease = true;

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

        public async Task<EnggBom> GetEnggBomByItemNoAndRevNo(string itemNumber, decimal revisionNumber)
        {
            var EnggBomDetailsbyItemNumber = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == itemNumber && x.RevisionNumber == revisionNumber && x.IsActive == true)
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
            // releaseEnggBom.LastModifiedBy = _createdBy;
            // releaseEnggBom.LastModifiedOn = DateTime.Now;
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
            decimal searchValueAsInt;
            bool isSearchValueNumeric = decimal.TryParse(searchParams.SearchValue, out searchValueAsInt);
            var enggBomDetails = FindAll().OrderByDescending(x => x.Id)
                          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                          || inv.ItemNumber.Contains(searchParams.SearchValue)
                          || inv.ReleaseNote.Contains(searchParams.SearchValue)
                          || inv.ReleaseFor.Contains(searchParams.SearchValue)
                || (isSearchValueNumeric && inv.ReleaseVersion.Equals(searchValueAsInt))
                          )));

            return PagedList<EngineeringBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        //public async Task<PagedList<EngineeringBom>> GetAllReleaseEnggBom([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var enggBomDetails = FindAll().OrderByDescending(x => x.Id)
        //                  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
        //                     inv.ReleaseNote.Contains(searchParams.SearchValue) || inv.ReleaseFor.Contains(searchParams.SearchValue))));

        //    return PagedList<EngineeringBom>.ToPagedList(enggBomDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

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
            // releaseCostBom.LastModifiedBy = _createdBy;
            // releaseCostBom.LastModifiedOn = DateTime.Now;
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
                ItemDescription = group.Select(bom => bom.ItemDescription).FirstOrDefault(),
                RevisionNumbers = group.Select(bom => bom.ReleaseVersion).ToArray(),
                ItemType = group.Select(bom => bom.ItemType).FirstOrDefault(),
            })
            .ToList();

            var releaseCostBomItemNumberList = releaseCostBomDetails
           .Select(bom => new CostingBomItemRevisionList
           {
               ItemNumber = bom.ItemNumber,
               ItemDescription = bom.ItemDescription,
               ReleaseVersion = bom.RevisionNumbers,
               ItemType = bom.ItemType
           }).ToList();

            return releaseCostBomItemNumberList;
        }
        public async Task<CostingBom> ReleasedCostBomByItemAndRevisionNumber(string itemNumber, decimal revisionNumber)
        {
            var releaseCostBom = await _tipsMasterDbContext.CostingBoms
            .Where(x => x.ItemNumber == itemNumber && x.ReleaseVersion == revisionNumber && x.IsActive == true)
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
             .Where(x => x.ItemNumber == itemNumber && x.IsActive == true).ToListAsync();

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
        //get latest production bom by

        //public async Task<int> GetLatestProBomCountByItemNumber(string itemNumber)
        //{
        //    var latestReleaseVersionsCount = await _tipsMasterDbContext.ProductionBoms
        //        .Where(x => x.ItemNumber == itemNumber)
        //        .CountAsync();

        //    return latestReleaseVersionsCount;
        //}
        public async Task<Dictionary<string, decimal>> GetSAsAndLatestVersion()
        {
            return await _tipsMasterDbContext.ProductionBoms.Where(x => x.ItemType == PartType.SA).GroupBy(p => p.ItemNumber).Select(g => new
            {
                ItemNumber = g.Key,
                LatestVersion = g.Max(p => p.ReleaseVersion)
            })
        .ToDictionaryAsync(x => x.ItemNumber, x => x.LatestVersion);
        }

        public async Task<Dictionary<string, decimal>> GetFGsAndLatestVersion()
        {
            return await _tipsMasterDbContext.ProductionBoms.Where(x => x.ItemType == PartType.FG).GroupBy(p => p.ItemNumber).Select(g => new
            {
                ItemNumber = g.Key,
                LatestVersion = g.Max(p => p.ReleaseVersion)
            })
        .ToDictionaryAsync(x => x.ItemNumber, x => x.LatestVersion);
        }
        public async Task<Dictionary<string, decimal>> GetSAsAndLatestVersionbyItemNo(string itemNumber)
        {
            return await _tipsMasterDbContext.ProductionBoms.Where(x => x.ItemType == PartType.SA && x.ItemNumber == itemNumber).GroupBy(p => p.ItemNumber).Select(g => new
            {
                ItemNumber = g.Key,
                LatestVersion = g.Max(p => p.ReleaseVersion)
            })
        .ToDictionaryAsync(x => x.ItemNumber, x => x.LatestVersion);
        }
        public async Task<List<ProductionBom>?> GetLatestProBomCountByItemNumber(string itemNumber)
        {
            List<ProductionBom>? latestReleaseVersionsCount = await _tipsMasterDbContext.ProductionBoms
                 .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                 .ToListAsync();

            return latestReleaseVersionsCount;
        }

        public async Task<int?> CreateReleaseProductBom(ProductionBom releaseProductBom)
        {
            releaseProductBom.CreatedBy = _createdBy;
            releaseProductBom.CreatedOn = DateTime.Now;
            var result = await Create(releaseProductBom);
            return result.Id;
        }

        public async Task<IEnumerable<object>> GetAllReleaseProductBomItemNumberVersionList()
        {
            var releaseProductBomDetails = _tipsMasterDbContext.CostingBoms
            .Where(x => x.IsReleaseCostCompleted == true && x.IsReleaseProductCompleted == false && x.IsActive == true)
            .GroupBy(bom => bom.ItemNumber)
            .Select(group => new
            {
                ItemNumber = group.Key,
                ItemDescription = group.Select(bom => bom.ItemDescription).FirstOrDefault(),
                RevisionNumbers = group.Select(bom => bom.ReleaseVersion).ToArray(),
                ItemType = group.Select(bom => bom.ItemType).FirstOrDefault(),
            })
            .ToList();

            var releaseProductBomItemNumberList = releaseProductBomDetails
           .Select(bom => new GetAllReleaseProductBomItemNumberVersionList
           {
               ItemNumber = bom.ItemNumber,
               ItemDescription = bom.ItemDescription,
               ReleaseVersion = bom.RevisionNumbers,
               ItemType = bom.ItemType
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


        public async Task<EnggBom> GetProductionBomByItemAndBomVersionNo(string itemNumber, decimal bomVersionNo)
        {
            var productionBomDetails = await _tipsMasterDbContext.EnggBoms
                                  .Where(x => x.ItemNumber == itemNumber && x.RevisionNumber == bomVersionNo && x.IsActive == true)
                                  .Include(x => x.EnggChildItems.Where(c => c.IsActive == true))
                                  .Include(x => x.NREConsumable)
                                  .FirstOrDefaultAsync();

            return productionBomDetails;
        }

        public async Task<ProductionBom> GetProductionBomByItemNumber(string itemNumber, decimal bomRevisonNumber)
        {
            var productionBomDetail = await _tipsMasterDbContext.ProductionBoms
                                    .Where(x => x.ItemNumber == itemNumber && x.ReleaseVersion == bomRevisonNumber && x.IsActive == true)
                                  .FirstOrDefaultAsync();

            return productionBomDetail;
        }

        //public async Task<decimal> GetLatestProductionBomByItemNumber(string itemNumber)
        //{

        //    decimal maxRevisionNumber = await _tipsMasterDbContext.ProductionBoms
        //    .Where(x => x.ItemNumber == itemNumber)
        //    .MaxAsync(x => x.ReleaseVersion);

        //    return maxRevisionNumber;
        //}

        public async Task<decimal> GetLatestProductionBomByItemNumber(string itemNumber)
        {
            decimal maxRevisionNumber = await _tipsMasterDbContext.ProductionBoms
                .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                .MaxAsync(x => (decimal?)x.ReleaseVersion) ?? -1;

            return maxRevisionNumber;
        }

        public async Task<IEnumerable<ReleaseProductionBomSPReport>> GetBOMReleaseSPReportWithParamForTrans(string? ItemNumber)
        {
            var result = _tipsMasterDbContext
            .Set<ReleaseProductionBomSPReport>()
            .FromSqlInterpolated($"CALL BomRelease_data({ItemNumber})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<ProductionBom>> GetAllProductionBomVersionListByItemNumber(string itemNumber)
        {
            var productionBomDetails = await _tipsMasterDbContext.ProductionBoms
               .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
             .ToListAsync();

            return productionBomDetails;
        }

        public async Task<IEnumerable<ProductionBomRevisionNumber>> GetAllProductionBomFGListByItemNumber(string itemNumber)
        {
            var latestReleaseVersion = _tipsMasterDbContext.ProductionBoms
             .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
             .OrderByDescending(x => x.ReleaseVersion)
             .Select(x => x.ReleaseVersion)
             .FirstOrDefault();

            var releaseProductBomDetails = new[] { latestReleaseVersion };


            var releaseProductBomItemNumberList = releaseProductBomDetails
               .Select(bom => new ProductionBomRevisionNumber
               {
                   ItemNumber = itemNumber,
                   ItemType = PartType.FG,
                   BomVersionNo = releaseProductBomDetails
               }).ToList();
            return releaseProductBomItemNumberList;


        }

        public async Task<ProductionBomRevisionNumberAndQty> GetAllProductionBomSAListByItemNumber(string itemNumber)
        {
            var releaseProductBomDetails = _tipsMasterDbContext.ProductionBoms
                 .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                 .Select(x => x.ReleaseVersion).ToArray();
            decimal requiredQty = 1;
            Dictionary<string, decimal> fgItemNumberListWithQty = new Dictionary<string, decimal>();
            fgItemNumberListWithQty = await GetFgItemNoListForAnSaItemNo(itemNumber, fgItemNumberListWithQty, requiredQty);

            var saItemDetailsWithRequiredQty = new ProductionBomRevisionNumberAndQty
            {
                ItemNumber = itemNumber,
                FGItemNumberWithSaBomQty = fgItemNumberListWithQty,
                ItemType = PartType.SA,
                BomVersionNo = releaseProductBomDetails
            };

            return saItemDetailsWithRequiredQty;
        }

        //private async Task<Dictionary<string, decimal>> GetFgItemNoListForAnSaItemNo(string itemNumber, Dictionary<string, decimal> fgItemNumberList, decimal requiredQty)
        //{
        //    var fgSaItemNoWithItemTypeDict = new Dictionary<string, Dictionary<int, PartType>>();

        //var enggBomIdsWithQty = await _tipsMasterDbContext.EnggChildItems
        //                        .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
        //                        .GroupBy(x => x.ItemNumber) // Group by ItemNumber
        //                        .Select(group => new
        //                        {
        //                            ItemNumber = group.Key,
        //                            BomId = group
        //                                .Select(x => x.EnggBomId)
        //                                .OrderByDescending(bomId => _tipsMasterDbContext.EnggBoms
        //                                    .Where(e => e.BOMId == bomId)
        //                                    .Max(e => e.RevisionNumber)
        //                                )
        //                                .FirstOrDefault(),
        //                            TotalQuantity = group
        //                                .OrderByDescending(x => _tipsMasterDbContext.EnggBoms
        //                                    .Where(e => e.BOMId == x.EnggBomId)
        //                                    .Max(e => e.RevisionNumber)
        //                                )
        //                                .Select(x => x.Quantity)
        //                                .FirstOrDefault()
        //                        })
        //                        .ToListAsync();

        //    var enggBomIds = enggBomIdsWithQty.Select(x => x.BomId).ToList();
        //    var enggBomIdsWithQtyDict = enggBomIdsWithQty.ToDictionary(x=> x.BomId,(x=>x.TotalQuantity * requiredQty));

        //    if (enggBomIdsWithQty.Count > 0)
        //    {
        //        var fgParents = await _tipsMasterDbContext.EnggBoms
        //            .Where(x => enggBomIds.Contains(x.BOMId))
        //            .Select(x => new { x.ItemNumber, x.ItemType, x.BOMId })
        //            .Distinct()
        //            .ToListAsync();

        //        foreach (var fgitem in fgParents)
        //        {
        //            Dictionary<int, PartType> bomIdWithItemTypeDict = new Dictionary<int, PartType>();

        //            if (!fgSaItemNoWithItemTypeDict.ContainsKey(fgitem.ItemNumber))
        //            {
        //                bomIdWithItemTypeDict.Add(fgitem.BOMId, fgitem.ItemType);
        //                fgSaItemNoWithItemTypeDict[fgitem.ItemNumber] = bomIdWithItemTypeDict;
        //            }
        //            else
        //            {
        //                fgSaItemNoWithItemTypeDict[fgitem.ItemNumber].Add(fgitem.BOMId, fgitem.ItemType);
        //            }
        //        }
        //    }


        //    foreach (var item in fgSaItemNoWithItemTypeDict)
        //    {
        //        var itemTypeDetails = item.Value;
        //        foreach (var itemType in itemTypeDetails)
        //        {
        //            if (itemType.Value == PartType.SA)
        //            {
        //                await GetFgItemNoListForAnSaItemNo(item.Key, fgItemNumberList, enggBomIdsWithQtyDict[itemType.Key]);
        //            }
        //            else
        //            {
        //                fgItemNumberList.Add(item.Key, enggBomIdsWithQtyDict[itemType.Key]);
        //            }
        //        }
        //    }

        //    return fgItemNumberList;
        //}
        private async Task<Dictionary<string, decimal>> GetFgItemNoListForAnSaItemNo(string itemNumber, Dictionary<string, decimal> fgItemNumberList, decimal requiredQty)
        {
            var fgSaItemNoWithItemTypeDict = new Dictionary<string, Dictionary<int, PartType>>();

            var enggBomIdsWithQty = await _tipsMasterDbContext.EnggChildItems
                                    .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
                                    //.GroupBy(x => x.ItemNumber) // Group by ItemNumber
                                    .Select(x => new
                                    {
                                        ItemNumber = x.ItemNumber,
                                        BomId = x.EnggBomId,
                                        //.Select(x => x.EnggBomId)
                                        //.OrderByDescending(bomId => _tipsMasterDbContext.EnggBoms
                                        //    .Where(e => e.BOMId == bomId)
                                        //    .Max(e => e.RevisionNumber)
                                        //)
                                        //.FirstOrDefault(),
                                        TotalQuantity = x.Quantity
                                        //.OrderByDescending(x => _tipsMasterDbContext.EnggBoms
                                        //    .Where(e => e.BOMId == x.EnggBomId)
                                        //    .Max(e => e.RevisionNumber)
                                        //)
                                        //.Select(x => x.Quantity)
                                        //.FirstOrDefault()
                                    })
                                    .ToListAsync();

            var enggBomIds = enggBomIdsWithQty.Select(x => x.BomId).ToList();
            var enggBomIdsWithQtyDict = enggBomIdsWithQty.ToDictionary(x => x.BomId, (x => x.TotalQuantity * requiredQty));

            if (enggBomIdsWithQty.Count > 0)
            {
                var fgParents = await _tipsMasterDbContext.EnggBoms
                    .Where(x => enggBomIds.Contains(x.BOMId) && x.IsActive == true)
                    .Select(x => new { x.ItemNumber, x.ItemType, x.BOMId, x.RevisionNumber })
                    .ToListAsync();

                var result = fgParents
                    .GroupBy(item => item.ItemNumber)
                    .Select(group => group.OrderByDescending(item => item.RevisionNumber).First())
                    .ToList();
                foreach (var fgitem in result)
                {
                    Dictionary<int, PartType> bomIdWithItemTypeDict = new Dictionary<int, PartType>();

                    if (!fgSaItemNoWithItemTypeDict.ContainsKey(fgitem.ItemNumber))
                    {
                        bomIdWithItemTypeDict.Add(fgitem.BOMId, fgitem.ItemType);
                        fgSaItemNoWithItemTypeDict[fgitem.ItemNumber] = bomIdWithItemTypeDict;
                    }
                    else
                    {
                        fgSaItemNoWithItemTypeDict[fgitem.ItemNumber].Add(fgitem.BOMId, fgitem.ItemType);
                    }
                }
            }


            foreach (var item in fgSaItemNoWithItemTypeDict)
            {
                var itemTypeDetails = item.Value;
                foreach (var itemType in itemTypeDetails)
                {
                    if (itemType.Value == PartType.SA)
                    {
                        await GetFgItemNoListForAnSaItemNo(item.Key, fgItemNumberList, enggBomIdsWithQtyDict[itemType.Key]);
                    }
                    else
                    {
                        // fgItemNumberList.Add(item.Key, enggBomIdsWithQtyDict[itemType.Key]);
                        if (!fgItemNumberList.ContainsKey(item.Key))
                        {
                            fgItemNumberList.Add(item.Key, enggBomIdsWithQtyDict[itemType.Key]);
                        }
                    }
                }
            }

            return fgItemNumberList;
        }



        public Task<IEnumerable<ProductionBom>> GetLatestProBomByItemNumber(string itemNumber)
        {
            throw new NotImplementedException();
        }


        //public async Task<IEnumerable<ProductionBomRevisionNumber>> GetAllProductionBomSAListByItemNumber(string itemNumber)
        //{
        //    var releaseProductBomDetails = _tipsMasterDbContext.ProductionBoms
        //         .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
        //         .Select(x => x.ReleaseVersion).ToArray();

        //    var enggChildItem = _tipsMasterDbContext.EnggChildItems
        //        .Where(x => x.ItemNumber == itemNumber && x.IsActive == true)
        //        .Select(x => x.EnggBomId).Distinct().ToList();

        //    Dictionary<string,PartType> fgSaItemNumberAndType = new Dictionary<string, PartType>();
        //    if (enggChildItem.Count > 0 && enggChildItem != null)
        //    {
        //        fgSaItemNumberAndType = _tipsMasterDbContext.EnggBoms
        //       .Where(x => enggChildItem.Contains(x.BOMId) && x.ItemType == PartType.FG)
        //       .Select(x => new { x.ItemNumber, x.ItemType }).Distinct().ToDictionaryAsync<string, PartType>();
        //    }



        //    foreach (var item in fgSaItemNumberAndType)
        //    {

        //    }

        //    var releaseProductBomItemNumberList = releaseProductBomDetails
        //       .Select(bom => new ProductionBomRevisionNumber
        //       {
        //           ItemNumber = itemNumber,
        //           FGItemNumber = fgSaItemNumberAndType,
        //           ItemType = PartType.SA,
        //           BomVersionNo = releaseProductBomDetails
        //       }).ToList();

        //    return releaseProductBomItemNumberList;
        //}

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
            // enggbomGroup.LastModifiedBy = _createdBy;
            //enggbomGroup.LastModifiedOn = DateTime.Now;
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
            //enggcustomFields.LastModifiedBy = _createdBy;
            // enggcustomFields.LastModifiedOn = DateTime.Now;
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
    public class EnggChildItemsRepository : RepositoryBase<EnggChildItem>, IEnggChildItemsRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public EnggChildItemsRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<string> UpdateEnggChilditems(EnggChildItem enggChildItem)
        {
            Update(enggChildItem);
            string result = $"EnggBomGroup Detail {enggChildItem.Id} is updated successfully!";
            return result;
        }
    }
}
