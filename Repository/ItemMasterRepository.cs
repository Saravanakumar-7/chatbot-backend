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
        public async Task<IEnumerable<GetDownloadUrlDtos>> GetDownloadUrlDetails(long itemMasterId)
        { 
            IEnumerable<GetDownloadUrlDtos> getDownloadDetails = await TipsMasterDbContext.imageUploads
                                .Where(b => b.Id == itemMasterId)
                                .Select(x => new GetDownloadUrlDtos()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath,
                                    FileByte=x.FileByte
                                })
                              .ToListAsync();

            return getDownloadDetails;
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
            .Include(d => d.ItemMasterWarehouse)
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
                .Include(M => M.ItemMasterRouting);
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
                             .Include(d => d.ItemMasterWarehouse)
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
                        &&(itemMasterSearch.ItemNumber.Any() ? itemMasterSearch.ItemNumber.Contains(item.ItemNumber) : true)
                        && (itemMasterSearch.Commodity.Any() ? itemMasterSearch.Commodity.Contains(item.Commodity) : true)
                        && (itemMasterSearch.MaterialGroup.Any() ? itemMasterSearch.MaterialGroup.Contains(item.MaterialGroup) : true)
                        && (itemMasterSearch.PurchaseGroup.Any() ? itemMasterSearch.PurchaseGroup.Contains(item.PurchaseGroup) : true)
                        && (itemMasterSearch.Department.Any() ? itemMasterSearch.Department.Contains(item.Department) : true))
                        .Include(t => t.ItemmasterAlternate)
                             .Include(t => t.ItemMasterApprovedVendor)
                             //.Include(t => t.ItemMasterFileUpload)
                             .Include(d => d.ItemMasterRouting)
                             .Include(d => d.ItemMasterWarehouse);
                }
                return query.ToList();
            }

        }
        public async Task<IEnumerable<ItemNoListDtos>> GetAllPurchasePartItemNoList()
        {
            IEnumerable<ItemNoListDtos> itemNumberListDto = await TipsMasterDbContext.ItemMasters
                               .Where(x => x.ItemType == PartType.PurchasePart || x.ItemType == PartType.TG)
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
                                .Where(c => c.ItemType == PartType.FG || c.ItemType == PartType.TG)
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,

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
                             .Include(d => d.ItemMasterWarehouse);
                }
                return query.ToList();
            }

        }

        public async Task<IEnumerable<ItemMaster>> GetAllFGItems()
        {
            var itemmasterFgDetails = FindAll().OrderByDescending(a => a.Id)
                .Where(inv => inv.ItemType == PartType.FG)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse);
            return itemmasterFgDetails;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllSAPurchasePartItems()
        {
            var itemmasterFgDetails = FindAll().OrderByDescending(a => a.Id).Where(inv => inv.ItemType == PartType.SA || inv.ItemType == PartType.PurchasePart)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse);
            return itemmasterFgDetails;
        }
         

        public async Task<IEnumerable<ItemMaster>> GetAllSAItems()
        {
            var itemmasterSADetails = FindAll().OrderByDescending(a => a.Id).Where(inv => inv.ItemType == PartType.SA)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse);
            return itemmasterSADetails;
        }
       public async Task<IEnumerable<ItemMaster>> GetAllFgSaItems()
        {
            var itemmasterSADetails = FindAll().OrderByDescending(a => a.Id).Where(inv => inv.ItemType == PartType.SA || inv.ItemType == PartType.FG)
            .Include(c => c.FileUpload)
            .Include(x => x.ImageUpload)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            //.Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse);
            return itemmasterSADetails;
        }
        //sa,fg, and fru

        public async Task<IEnumerable<ItemMaster>> GetAllFgSaFruItems()
        {
            var itemmasterFgSaFRUDetails = FindAll().OrderByDescending(a => a.Id)
               .Where(a=>a.ItemType == PartType.SA || a.ItemType == PartType.FG || a.ItemType == PartType.FRU)
                         //.Include(c => c.FileUpload)
                          //  .Include(x => x.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                //.Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse).ToList();
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
                            .Include(p => p.ItemMasterWarehouse).FirstOrDefaultAsync();


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
                                .Where(x=>x.IsActive == true)
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
            IEnumerable<ItemMasterIdNoListDto> itemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,

                                })
                              .ToListAsync();

            return itemMasterIdNoListDto;
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
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber)
                // .Include(c => c.FileUpload)
               // .Include(x => x.ImageUpload)
                 .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                //.Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)

                             .FirstOrDefaultAsync();
            return getItemMasterByItemNo;
        }

        public async Task<List<ItemWithPartTypeDto>> GetItemPartTypeByItemNo(List<string> ItemNumberList)
        {
            var itemWithPartTypes = await FindByCondition(x => ItemNumberList.Contains(x.ItemNumber))
                .Select(s => new ItemWithPartTypeDto
                {
                    ItemNumber = s.ItemNumber,
                    PartType = s.ItemType
                }
                ).ToListAsync();
            return itemWithPartTypes;
        }

        public async Task<List<ItemMasterMtrPartNoDto>> GetItemMasterByPartNo(string partNumber)
        {
            var itemMasterDescription = await TipsMasterDbContext.ItemMasters
                                .Where(x => x.ItemNumber == partNumber)
                                .Select(m => m.Description).FirstOrDefaultAsync();

            var itemMasterUom = await TipsMasterDbContext.ItemMasters
                               .Where(x => x.ItemNumber == partNumber)
                               .Select(m => m.Uom).FirstOrDefaultAsync();

            var itemMasterId = await TipsMasterDbContext.ItemMasters
                                .Where(x => x.ItemNumber == partNumber)
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
                                    FilePath = x.FilePath
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
