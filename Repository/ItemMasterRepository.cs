using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Repository
{
    public class ItemMasterRepository : RepositoryBase<ItemMaster>, IItemMasterRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ItemMasterRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<long> CreateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.CreatedBy = _createdBy;
            itemMaster.CreatedOn = DateTime.Now;
            // itemMaster.LastModifiedBy = _createdBy;
            // itemMaster.LastModifiedOn = DateTime.Now;
            itemMaster.Unit = _unitname;
            var result = await Create(itemMaster);

            return result.Id;
        }
        public async Task<GetDownloadUrlDtos> GetDownloadUrlDetails(long itemMasterId)
        {
            GetDownloadUrlDtos? getDownloadDetails = await TipsMasterDbContext.imageUploads.Where(b => b.Id == itemMasterId)
                                 .Select(x => new GetDownloadUrlDtos()
                                 {
                                     Id = x.Id,
                                     FileName = x.FileName,
                                     FileExtension = x.FileExtension,
                                     FilePath = x.FilePath,
                                     FileByte = x.FileByte
                                 }).FirstOrDefaultAsync();

            return getDownloadDetails;
        }
        public async Task<List<GetDownloadUrlswithitemnumber>> GetImageDetails(Dictionary<string, int?> itemImageids)
        {
            List<GetDownloadUrlswithitemnumber> downloadDetailsList = new List<GetDownloadUrlswithitemnumber>();
            foreach (var itemdetails in itemImageids)
            {
                string itemNumber = itemdetails.Key;
                int? imageId = itemdetails.Value;
                GetDownloadUrlswithitemnumber? getDownloadDetails = await TipsMasterDbContext.imageUploads
                .Where(b => b.Id == imageId).Select(x => new GetDownloadUrlswithitemnumber
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    FileExtension = x.FileExtension
                })
                .FirstOrDefaultAsync();

                if (getDownloadDetails != null)
                {
                    getDownloadDetails.Itemnumber = itemNumber;
                    downloadDetailsList.Add(getDownloadDetails);
                }
            }
            return downloadDetailsList;
        }
        public async Task<string> DeleteItemMaster(ItemMaster itemMaster)
        {
            Delete(itemMaster);
            string result = $"ItemMaster details of {itemMaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllActiveItemMasters()
        {
            var allActiveCompanyMasters = await FindByCondition(x => x.IsActive == true)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts)
            .ToListAsync();
            return allActiveCompanyMasters;
        }
        public async Task<PagedList<ItemMaster>> GetAllItemMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)     // inv.ItemNumber.Contains(searchParams.SearchValue)
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

            var itemMasterDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv =>
                    (string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
                    inv.ItemNumber.Contains(searchParams.SearchValue) ||
                    inv.Description.Contains(searchParams.SearchValue) ||
                     inv.Commodity.Contains(searchParams.SearchValue) ||
                     inv.MaterialGroup.Contains(searchParams.SearchValue)
                     || inv.ItemType.Equals(check)))
                .Include(x => x.ItemmasterAlternate)
                .Include(M => M.ItemMasterWarehouse)
                .Include(M => M.ItemMasterApprovedVendor)
                .Include(M => M.ItemMasterRouting)
                .Include(M => M.ItemMasterSchedules)
                .ThenInclude(x => x.ItemMasterScheduleParts);
            return PagedList<ItemMaster>.ToPagedList(itemMasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        //public async Task<PagedList<ItemMaster>> GetAllItemMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)     // inv.ItemNumber.Contains(searchParams.SearchValue)
        //{
        //    int searchValueInt;
        //    bool isSearchValueInt = int.TryParse(searchParams.SearchValue, out searchValueInt);

        //    var itemMasterDetails = FindAll().OrderByDescending(x => x.Id)
        //        .Where(inv =>
        //            (string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
        //            inv.ItemNumber.Contains(searchParams.SearchValue) ||
        //            inv.Description.Contains(searchParams.SearchValue) ||
        //             inv.Commodity.Contains(searchParams.SearchValue) ||
        //             inv.MaterialGroup.Contains(searchParams.SearchValue)))
        //        //(Enum.TryParse(searchParams.SearchValue, out Ite itemType) && inv.ItemType == itemType)))

        //        .Include(x => x.ItemmasterAlternate)
        //        .Include(M => M.ItemMasterWarehouse)
        //        .Include(M => M.ItemMasterApprovedVendor)
        //        .Include(M => M.ItemMasterRouting);




        //    return PagedList<ItemMaster>.ToPagedList(itemMasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
        //    public async Task<PagedList<ItemMaster>> GetAllItemMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //    {
        //    int? searchValueAsInt = null;
        //    if (!string.IsNullOrWhiteSpace(searchParams.SearchValue) && int.TryParse(searchParams.SearchValue, out int intValue))
        //    {
        //        searchValueAsInt = intValue;
        //    }

        //    // Start with the query for itemmasters
        //    var query = FindAll().AsQueryable();

        //    // Apply filtering
        //    if (!string.IsNullOrWhiteSpace(searchParams.SearchValue))
        //    {
        //        query = query.Where(inv =>
        //            inv.ItemNumber.Contains(searchParams.SearchValue) ||
        //            inv.Description.Contains(searchParams.SearchValue) ||
        //            (searchValueAsInt != null && inv.ItemType.Equals(searchValueAsInt)) ||
        //            inv.Commodity.Contains(searchParams.SearchValue) ||
        //            inv.MaterialGroup.Contains(searchParams.SearchValue)
        //        );
        //    }

        //    // Apply ordering
        //    query = query.OrderByDescending(x => x.Id);

        //    // Include related entities
        //    query = query
        //        .Include(t => t.ItemmasterAlternate)
        //        .Include(t => t.ItemMasterApprovedVendor)
        //        .Include(d => d.ItemMasterRouting)
        //        .Include(d => d.ItemMasterWarehouse);

        //    // Execute the query and return the paged list
        //    return PagedList<ItemMaster>.ToPagedList(query, pagingParameter.PageNumber, pagingParameter.PageSize);

        //    //var itemmasterDetails = FindAll().OrderByDescending(x => x.Id)
        //    //     .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
        //    // inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue))
        //    // || inv.Commodity.Contains(searchParams.SearchValue) || inv.MaterialGroup.Contains(searchParams.SearchValue))))
        //    //              .Include(t => t.ItemmasterAlternate)
        //    //             .Include(t => t.ItemMasterApprovedVendor)
        //    //              .Include(d => d.ItemMasterRouting)
        //    //              .Include(d => d.ItemMasterWarehouse);

        //    //return PagedList<ItemMaster>.ToPagedList(itemmasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<ItemMaster>> SearchItemMasterDate([FromQuery] SearchDateParamess searchDateParam)
        {
            var itemMasterDetails = TipsMasterDbContext.ItemMasters
                             .Where(inv => ((inv.CreatedOn >= searchDateParam.SearchFromDate &&
                                inv.CreatedOn <= searchDateParam.SearchToDate
                                )))
                             .Include(t => t.ItemmasterAlternate)
                             .Include(t => t.ItemMasterApprovedVendor)
                             //.Include(t => t.ItemMasterFileUpload)
                             .Include(d => d.ItemMasterRouting)
                             .Include(d => d.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts)
                             .ToList();
            return itemMasterDetails;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllItemMasterWithItems(ItemMasterSearchDto itemMasterSearch)
        {
            using (var context = TipsMasterDbContext)
            {
                var query = TipsMasterDbContext.ItemMasters.Include("ItemmasterAlternate")
                    .Include("ItemMasterApprovedVendor").Include("ItemMasterRouting")
                    .Include("ItemMasterWarehouse");
                if (itemMasterSearch != null || (itemMasterSearch.ItemNumber.Any())
                    && itemMasterSearch.ItemType.Any() && itemMasterSearch.Commodity.Any() && itemMasterSearch.MaterialGroup.Any()
                    && itemMasterSearch.PurchaseGroup.Any()
                    && itemMasterSearch.Department.Any())

                {
                    query = query.Where
                        (item => (itemMasterSearch.ItemType.Any() ? itemMasterSearch.ItemType.Contains(item.ItemType) : true)
                        && (itemMasterSearch.ItemNumber.Any() ? itemMasterSearch.ItemNumber.Contains(item.ItemNumber) : true)
                        && (itemMasterSearch.Commodity.Any() ? itemMasterSearch.Commodity.Contains(item.Commodity) : true)
                        && (itemMasterSearch.MaterialGroup.Any() ? itemMasterSearch.MaterialGroup.Contains(item.MaterialGroup) : true)
                        && (itemMasterSearch.PurchaseGroup.Any() ? itemMasterSearch.PurchaseGroup.Contains(item.PurchaseGroup) : true)
                        && (itemMasterSearch.Department.Any() ? itemMasterSearch.Department.Contains(item.Department) : true))
                        .Include(t => t.ItemmasterAlternate)
                             .Include(t => t.ItemMasterApprovedVendor)
                             //.Include(t => t.ItemMasterFileUpload)
                             .Include(d => d.ItemMasterRouting)
                             .Include(d => d.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts);
                }
                return query.ToList();
            }

        }
        public async Task<IEnumerable<ItemNoListDtos>> GetAllOnlyServiceItemsPurchasePartItemNoList()
        {
            IEnumerable<ItemNoListDtos> itemNumberListDto = await TipsMasterDbContext.ItemMasters
                               .Where(x => (x.ItemType == PartType.PurchasePart || x.ItemType == PartType.TG) && x.IsActive == true && x.PoMaterialType== "ServiceItem")
                               .Select(c => new ItemNoListDtos()
                               {
                                   ItemNumber = c.ItemNumber,
                                   Description = c.Description,
                               })
                             .ToListAsync();

            return itemNumberListDto;
        }
        public async Task<IEnumerable<ItemNoListDtos>> GetAllPurchasePartItemNoListExcludingServiceItems()
        {
            IEnumerable<ItemNoListDtos> itemNumberListDto = await TipsMasterDbContext.ItemMasters
                               .Where(x => (x.ItemType == PartType.PurchasePart || x.ItemType == PartType.TG) && x.IsActive == true && x.PoMaterialType != "ServiceItem")
                               .Select(c => new ItemNoListDtos()
                               {
                                   ItemNumber = c.ItemNumber,
                                   Description = c.Description,
                               })
                             .ToListAsync();

            return itemNumberListDto;
        }
        public async Task<IEnumerable<ItemNoListDtos>> GetAllPurchasePartItemNoList()
        {
            IEnumerable<ItemNoListDtos> itemNumberListDto = await TipsMasterDbContext.ItemMasters
                               .Where(x => (x.ItemType == PartType.PurchasePart || x.ItemType == PartType.TG) && x.IsActive == true)
                               .Select(c => new ItemNoListDtos()
                               {
                                   ItemNumber = c.ItemNumber,
                                   Description = c.Description,
                               })
                             .ToListAsync();

            return itemNumberListDto;
        }
        public async Task<IEnumerable<ItemNoListDtos>> GetAllIsPRRequiredStatusTruePPItemNoList()
        {
            IEnumerable<ItemNoListDtos> itemNumberListDto = await TipsMasterDbContext.ItemMasters
                               .Where(x => (x.ItemType == PartType.PurchasePart || x.ItemType == PartType.TG) && x.IsPRRequired == true && x.IsActive == true)
                               .Select(c => new ItemNoListDtos()
                               {
                                   ItemNumber = c.ItemNumber,
                                   Description = c.Description,
                               })
                             .ToListAsync();

            return itemNumberListDto;
        }

        public async Task<IEnumerable<ItemNoListDtos>> GetAllIsPRRequiredStatusTrueKITItemNoList()
        {
            IEnumerable<ItemNoListDtos> itemNumberListDto = await TipsMasterDbContext.ItemMasters
                               .Where(x => (x.ItemType == PartType.Kit) && x.IsPRRequired == true && x.IsActive == true)
                               .Select(c => new ItemNoListDtos()
                               {
                                   ItemNumber = c.ItemNumber,
                                   Description = c.Description,
                               })
                             .ToListAsync();

            return itemNumberListDto;
        }

        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllFgTgItemMasterItemNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> getAllActiveItemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters
                                .Where(c => (c.ItemType == PartType.FG || c.ItemType == PartType.TG) && c.IsActive == true)
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,
                                    PartType = c.ItemType
                                })
                              .ToListAsync();

            return getAllActiveItemMasterIdNoListDto;
        }

        public async Task<IEnumerable<ItemMaster>> SearchItemMaster([FromQuery] SearchParames searchParames)
        {
            using (var context = TipsMasterDbContext)
            {
                var query = TipsMasterDbContext.ItemMasters.Include("ItemmasterAlternate")
                    .Include("ItemMasterApprovedVendor").Include("ItemMasterRouting")
                    .Include("ItemMasterWarehouse");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(itm => itm.ItemNumber.Contains(searchParames.SearchValue)
                || itm.Description.Contains(searchParames.SearchValue) ||
                itm.MaterialGroup.Contains(searchParames.SearchValue) ||
                itm.Commodity.Contains(searchParames.SearchValue))
                        .Include(t => t.ItemmasterAlternate)
                             .Include(t => t.ItemMasterApprovedVendor)
                             //.Include(t => t.ItemMasterFileUpload)
                             .Include(d => d.ItemMasterRouting)
                             .Include(d => d.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts);
                }
                return query.ToList();
            }

        }

        public async Task<IEnumerable<ItemMaster>> GetAllFGItems()
        {
            var itemmasterFgDetails = FindAll().OrderByDescending(a => a.Id)
                .Where(inv => (inv.ItemType == PartType.FG) && inv.IsActive == true)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts);
            return itemmasterFgDetails;
        }
        public async Task<IEnumerable<string>> GetAllFGItemNumberList()
        {
            var itemmasterFgItemNumbers = FindAll().OrderByDescending(a => a.Id)
                .Where(inv => (inv.ItemType == PartType.FG) && inv.IsActive == true)
                .Select(x => x.ItemNumber)
                .ToList();

            return itemmasterFgItemNumbers;
        }
        public async Task<IEnumerable<ItemMaster>> GetAllSAPurchasePartItems()
        {

            var itemmasterFgDetails = await TipsMasterDbContext.ItemMasters
                                       .Where(inv => (inv.ItemType == PartType.SA || inv.ItemType == PartType.PurchasePart || inv.ItemType == PartType.Kit) && inv.IsActive == true)
                                       .OrderByDescending(inv => inv.Id)
                                       .Include(inv => inv.ItemmasterAlternate)
                                       .Include(inv => inv.ItemMasterApprovedVendor)
                                       .Include(inv => inv.ItemMasterRouting)
                                       .Include(inv => inv.ItemMasterWarehouse)
                                       .Include(inv => inv.ItemMasterSchedules)
                                            .ThenInclude(x => x.ItemMasterScheduleParts)
                                       .ToListAsync();  // Ensure the query is executed asynchronously

            return itemmasterFgDetails;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllKitComponentItemList()
        {
            var itemmasterKitComponentDetails = await TipsMasterDbContext.ItemMasters
                                   .Where(inv => (inv.ItemType == PartType.KitComponent) && inv.IsActive == true)
                                   .OrderByDescending(a => a.Id)
                                   .Include(t => t.ItemmasterAlternate)
                                   .Include(t => t.ItemMasterApprovedVendor)
                                   .Include(d => d.ItemMasterRouting)
                                   .Include(d => d.ItemMasterWarehouse)
                                   .Include(inv => inv.ItemMasterSchedules)
                                        .ThenInclude(x => x.ItemMasterScheduleParts)
                                   .ToListAsync();

            return itemmasterKitComponentDetails;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllSAItems()
        {
            var itemmasterSADetails = FindAll().OrderByDescending(a => a.Id).Where(inv => (inv.ItemType == PartType.SA) && inv.IsActive == true)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts);
            return itemmasterSADetails;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllKITItems()
        {
            var itemmasterSADetails = FindAll().OrderByDescending(a => a.Id).Where(inv => (inv.ItemType == PartType.Kit) && inv.IsActive == true)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts);
            return itemmasterSADetails;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllFgSaItems()
        {
            var itemmasterSADetails = FindAll().OrderByDescending(a => a.Id).Where(inv => (inv.ItemType == PartType.SA || inv.ItemType == PartType.FG) && inv.IsActive == true);

            return itemmasterSADetails;
        }
        //sa,fg, and fru

        public async Task<IEnumerable<ItemMaster>> GetAllFgSaFruItems()
        {
            var itemmasterFgSaFRUDetails = FindAll().OrderByDescending(a => a.Id)
               .Where(a => (a.ItemType == PartType.SA || a.ItemType == PartType.FG || a.ItemType == PartType.FRU || a.ItemType == PartType.Kit) && a.IsActive == true)
                            //.Include(c => c.FileUpload)
                            //  .Include(x => x.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                //.Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts).ToList();
            return itemmasterFgSaFRUDetails;


        }

        public async Task<ItemMaster> GetItemMasterById(int id)
        {
            var getItemMasterById = await TipsMasterDbContext.ItemMasters
                            .Where(x => x.Id == id)
                            //.Include(c=>c.FileUpload)
                            //.Include(b=>b.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                            .Include(x => x.ItemMasterApprovedVendor)
                            //.Include(m => m.ItemMasterFileUpload)
                            .Include(s => s.ItemMasterRouting)
                            .Include(p => p.ItemMasterWarehouse).Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts).FirstOrDefaultAsync();


            return getItemMasterById;
        }

        public async Task<string> UpdateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.LastModifiedBy = _createdBy;
            itemMaster.LastModifiedOn = DateTime.Now;
            Update(itemMaster);
            string result = $"ItemMaster details of {itemMaster.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> getAllActiveItemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters
                                .Where(x => x.IsActive == true)
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,

                                })
                              .ToListAsync();

            return getAllActiveItemMasterIdNoListDto;
        }
        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveAndInActiveItemMasterIdNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> getAllActiveItemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,

                                })
                              .ToListAsync();

            return getAllActiveItemMasterIdNoListDto;
        }

        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllItemMasterIdNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> itemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters.Where(x => x.IsActive == true)
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,
                                    Uom = c.Uom
                                })
                              .ToListAsync();

            return itemMasterIdNoListDto;
        }

        public async Task<IEnumerable<ItemMasterAlterMtrPartNoDto>> GetAllItemMasterMftrNoList()
        {
            var itemMasterId = await TipsMasterDbContext.ItemMasters.Where(x => x.IsActive == true).Select(x => x.Id).ToListAsync();

            IEnumerable<ItemMasterAlterMtrPartNoDto> itemMasterIdNoListDto = await TipsMasterDbContext.ItemmasterAlternates
                            .Where(x => itemMasterId.Contains(x.ItemMasterId))
                            .GroupBy(g => new { g.ManufacturerPartNo, g.Manufacturer })
                              .Select(c => new ItemMasterAlterMtrPartNoDto()
                              {
                                  ManufacturerPartNo = c.Key.ManufacturerPartNo,
                                  Manufacturer = c.Key.Manufacturer
                              })
                              .ToListAsync();

            return itemMasterIdNoListDto;
        }

        //public async Task<IEnumerable<ItemMasterAlterMtrPartNoDto>> GetAllItemMasterMftrNoList()
        //{
        //    var result = await (from alt in TipsMasterDbContext.ItemmasterAlternates
        //                        join master in TipsMasterDbContext.ItemMasters
        //                            on alt.ItemMasterId equals master.Id
        //                        where master.IsActive
        //                        group alt by new { alt.ManufacturerPartNo, alt.Manufacturer } into g
        //                        select new ItemMasterAlterMtrPartNoDto
        //                        {
        //                            ManufacturerPartNo = g.Key.ManufacturerPartNo,
        //                            Manufacturer = g.Key.Manufacturer
        //                        }).ToListAsync();

        //    return result;
        //}




        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllOpenGrinStatusTrueItemMasterIdNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> itemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters
                                .Where(x => x.OpenGrin == true && x.IsActive == true)
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,
                                    Uom = c.Uom
                                })
                              .ToListAsync();

            return itemMasterIdNoListDto;
        }

        public async Task<IEnumerable<ItemNoListDtos>> GetAllActiveItemNumberListbyPartType(PartType partType)
        {
            IEnumerable<ItemNoListDtos> itemMasterNoListDto = await TipsMasterDbContext.ItemMasters
                                .Where(x => x.ItemType == partType && x.IsActive == true)
                                .Select(c => new ItemNoListDtos()
                                {
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description
                                })
                              .ToListAsync();

            return itemMasterNoListDto;
        }

        public async Task<IEnumerable<FileUpload>> GetAllItemMasterFileUploadList(string itemNumber)
        {
            IEnumerable<FileUpload> itemMasterFileUploadList = await TipsMasterDbContext.fileUploads
                            .Where(x => x.ParentId == itemNumber)
                                .Select(c => new FileUpload()
                                {
                                    Id = c.Id,
                                    FileName = c.FileName,
                                    FileExtension = c.FileExtension,
                                    FilePath = c.FilePath,
                                    FileByte = c.FileByte,
                                    DocumentFrom = c.DocumentFrom,
                                    ParentId = c.ParentId,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedOn,
                                    LastModifiedBy = c.LastModifiedBy,
                                    LastModifiedOn = c.LastModifiedOn,

                                })
                              .ToListAsync();

            return itemMasterFileUploadList;
        }
        public async Task<ItemMaster> GetItemMasterByItemNumber(string ItemNumber)
        {
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber && x.IsActive == true)
                 // .Include(c => c.FileUpload)
                 // .Include(x => x.ImageUpload)
                 .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                //.Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)
                                .Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts)
                             .FirstOrDefaultAsync();
            return getItemMasterByItemNo;
        }
        public async Task<ItemMaster> GetItemMasterDetailsByItemNumber(string ItemNumber)
        {
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber)
                 // .Include(c => c.FileUpload)
                 // .Include(x => x.ImageUpload)
                 .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                //.Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)
                                .Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts)
                             .FirstOrDefaultAsync();
            return getItemMasterByItemNo;
        }
        public async Task<List<ItemMaster>> GetItemDetailsByItemNumberList(List<string> ItemNumbers)
        {
            var getItemMasterByItemNo = await FindByCondition(x => ItemNumbers.Contains(x.ItemNumber) && x.IsActive == true).ToListAsync();
            return getItemMasterByItemNo;
        }

        public async Task<ItemMaster> GetItemMasterByItemNumberAndPartType(string ItemNumber, PartType partType)
        {
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber && x.IsActive == true && x.ItemType == partType)

                 .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)
                                .Include(M => M.ItemMasterSchedules).ThenInclude(x => x.ItemMasterScheduleParts)
                             .FirstOrDefaultAsync();
            return getItemMasterByItemNo;
        }

        public async Task<bool> CheckItemMasterExists(string itemnumber)
        {
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == itemnumber).FirstOrDefaultAsync();
            return getItemMasterByItemNo != null;
        }
        public async Task<Dictionary<string, int?>> GetItemsImageIds(List<string> ItemNumbers)
        {
            Dictionary<string, int?> imageIds = await FindAll().Where(x => ItemNumbers.Contains(x.ItemNumber) && x.IsActive == true && x.ImageUpload != null).Select(x => new { x.ItemNumber, x.ImageUpload }).ToDictionaryAsync(x => x.ItemNumber, x => x.ImageUpload);
            return imageIds;
        }
        public async Task<string> GetClosedIqcItemMasterItemNo(string ItemNumber)
        {
            var iqcCloseditemMasterItemNoList = await FindByCondition(x => x.ItemNumber == ItemNumber && x.IsIQCRequired == false && x.IsActive == true)
                             .Select(x => x.ItemNumber)
                             .FirstOrDefaultAsync();

            return iqcCloseditemMasterItemNoList;
        }

        public async Task<List<ItemWithPartTypeDto>> GetItemPartTypeByItemNo(List<string> ItemNumberList)
        {
            var itemWithPartTypes = await FindByCondition(x => ItemNumberList.Contains(x.ItemNumber) && x.IsActive == true)
                .SelectMany(s => s.ItemmasterAlternate.Where(x => x.IsDefault == true).Select(a => new ItemWithPartTypeDto
                {
                    ItemNumber = s.ItemNumber,
                    PartType = s.ItemType,
                    MftrItemNumber = a.ManufacturerPartNo,
                    MaterialGroup = s.MaterialGroup
                }))
                .ToListAsync();

            return itemWithPartTypes;
        }

        public async Task<List<ItemWithPartTypeAndMinDto>> GetItemMasterPartTypeAndMinByItemNumber(List<string> ItemNumberList)
        {
            var itemWithPartTypes = await FindByCondition(x => ItemNumberList.Contains(x.ItemNumber) && x.IsActive == true)
                .Select(s => new ItemWithPartTypeAndMinDto
                {
                    ItemNumber = s.ItemNumber,
                    PartType = s.ItemType,
                    Min = s.Min,
                    UOM = s.Uom
                }
                ).ToListAsync();
            return itemWithPartTypes;
        }
        public async Task<List<ItemMasterMtrPartNoDto>> GetItemMasterByPartNo(string partNumber)
        {
            var itemMasterDescription = await TipsMasterDbContext.ItemMasters
                                .Where(x => x.ItemNumber == partNumber && x.IsActive == true)
                                .Select(m => m.Description).FirstOrDefaultAsync();

            var itemMasterUom = await TipsMasterDbContext.ItemMasters
                               .Where(x => x.ItemNumber == partNumber && x.IsActive == true)
                               .Select(m => m.Uom).FirstOrDefaultAsync();

            var itemMasterId = await TipsMasterDbContext.ItemMasters
                                .Where(x => x.ItemNumber == partNumber && x.IsActive == true)
                                .Select(m => m.Id).FirstOrDefaultAsync();

            var itemMasterDetails = await TipsMasterDbContext.ItemmasterAlternates
                                .Where(m => itemMasterId == m.Id && m.IsDefault == true)
                                .Select(s => new ItemMasterMtrPartNoDto()
                                {
                                    ManufacturerPartNo = s.ManufacturerPartNo,
                                    Description = itemMasterDescription,
                                    Uom = itemMasterUom

                                }).ToListAsync();

            return itemMasterDetails;

        }
    }

    public class FileUploadDocumentRepository : RepositoryBase<FileUpload>, IFileUploadRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public FileUploadDocumentRepository(TipsMasterDbContext tipsMasterDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsMasterDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateFileUploadDocument(FileUpload fileUpload)
        {
            fileUpload.CreatedBy = _createdBy;
            fileUpload.CreatedOn = DateTime.Now;
            // fileUpload.LastModifiedBy = _createdBy;
            // fileUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(fileUpload);
            return result.Id;
        }
        public async void Delete(FileUpload fileUpload)
        {
            TipsMasterDbContext.fileUploads.Remove(fileUpload);
        }
        public async Task<FileUpload> GetFileUploadByIdAsync(int id)
        {
            return await TipsMasterDbContext.fileUploads.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<int?> CreateImageUploadDocument(FileUpload fileUpload)
        {
            fileUpload.CreatedBy = _createdBy;
            fileUpload.CreatedOn = DateTime.Now;
            // fileUpload.LastModifiedBy = _createdBy;
            //fileUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(fileUpload);
            return result.Id;
        }
        public async Task<List<FileUploadDto>> GetDownloadUrlDetails(string FileIds)
        {
            List<FileUploadDto> fileUploads = new List<FileUploadDto>();
            if (FileIds != null)
            {
                string[]? ids = FileIds.Split(',');

                for (int i = 0; i < ids.Count(); i++)
                {
                    FileUploadDto getDownloadDetails = await TipsMasterDbContext.fileUploads
                                .Where(b => b.Id == Convert.ToInt32(ids[i]))
                                .Select(x => new FileUploadDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath,
                                    FileByte = x.FileByte
                                }).FirstOrDefaultAsync();
                    if (getDownloadDetails != null)
                        fileUploads.Add(getDownloadDetails);
                }
            }
            return fileUploads;
        }
    }

    public class ImageUploadDocumentRepository : RepositoryBase<ImageUpload>, IImageUploadRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ImageUploadDocumentRepository(TipsMasterDbContext tipsMasterDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsMasterDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task DeleteImage(int Id)
        {
            // Retrieve the record to delete
            var imageToDelete = TipsMasterDbContext.imageUploads.FirstOrDefault(i => i.Id == Id);
            if (imageToDelete != null)
            {
                // Remove the record from the context and then save changes
                TipsMasterDbContext.imageUploads.Remove(imageToDelete);
                //TipsMasterDbContext.SaveChanges();
            }
        }
        public async Task<int?> ImageUploadDocument(ImageUpload imageUpload)
        {
            imageUpload.CreatedBy = _createdBy;
            imageUpload.CreatedOn = DateTime.Now;
            //  imageUpload.LastModifiedBy = _createdBy;
            // imageUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(imageUpload);
            return result.Id;
        }
        public async Task<string?> GetImageFileByte(string filename)
        {
            var fileByte = TipsMasterDbContext.imageUploads.Where(x => x.FileName == filename).Select(x => x.FileByte).FirstOrDefault();
            if (fileByte != null)
            {
                return fileByte.ToString();
            }
            return null;
        }

    }

}
