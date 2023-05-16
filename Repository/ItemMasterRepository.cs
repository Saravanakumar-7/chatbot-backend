using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Repository
{
    public class ItemMasterRepository : RepositoryBase<ItemMaster>, IItemMasterRepository
    {
        public ItemMasterRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<long> CreateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.CreatedBy = "Admin";
            itemMaster.CreatedOn = DateTime.Now;
            itemMaster.Unit = "Bangalore";
            var result = await Create(itemMaster);

            return result.Id;
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
            .Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse)
            .ToListAsync();
            return allActiveCompanyMasters;
        }

        public async Task<PagedList<ItemMaster>> GetAllItemMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var itemmasterDetails = FindAll().OrderByDescending(x => x.Id)
                 .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
             inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue))
             || inv.Commodity.Contains(searchParams.SearchValue) || inv.MaterialGroup.Contains(searchParams.SearchValue))))
                          .Include(t => t.ItemmasterAlternate)
                         .Include(t => t.ItemMasterApprovedVendor)
                         .Include(t => t.ItemMasterFileUpload)
                          .Include(d => d.ItemMasterRouting)
                          .Include(d => d.ItemMasterWarehouse);

            return PagedList<ItemMaster>.ToPagedList(itemmasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<ItemMaster>> SearchItemMasterDate([FromQuery] SearchDateParamess searchDateParam)
        {
            var itemMasterDetails = TipsMasterDbContext.ItemMasters
                             .Where(inv => ((inv.CreatedOn >= searchDateParam.SearchFromDate &&
                                inv.CreatedOn <= searchDateParam.SearchToDate
                                )))
                             .Include(t => t.ItemmasterAlternate)
                             .Include(t => t.ItemMasterApprovedVendor)
                             .Include(t => t.ItemMasterFileUpload)
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
                    .Include("ItemMasterApprovedVendor").Include("ItemMasterFileUpload").Include("ItemMasterRouting")
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
                        && (itemMasterSearch.Department.Any() ? itemMasterSearch.Department.Contains(item.Department) : true));
                }
                return query.ToList();
            }

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
                    .Include("ItemMasterApprovedVendor").Include("ItemMasterFileUpload").Include("ItemMasterRouting")
                    .Include("ItemMasterWarehouse");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(itm => itm.ItemNumber.Contains(searchParames.SearchValue)
                || itm.Description.Contains(searchParames.SearchValue) ||
                itm.MaterialGroup.Contains(searchParames.SearchValue) ||
                itm.Commodity.Contains(searchParames.SearchValue));
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
            .Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse);
            return itemmasterFgDetails;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllSAPurchasePartItems()
        {
            var itemmasterFgDetails = FindAll().OrderByDescending(a => a.Id).Where(inv => inv.ItemType == PartType.SA || inv.ItemType == PartType.PurchasePart)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            .Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse);
            return itemmasterFgDetails;
        }
         

        public async Task<IEnumerable<ItemMaster>> GetAllSAItems()
        {
            var itemmasterSADetails = FindAll().OrderByDescending(a => a.Id).Where(inv => inv.ItemType == PartType.SA)
            .Include(t => t.ItemmasterAlternate)
            .Include(t => t.ItemMasterApprovedVendor)
            .Include(t => t.ItemMasterFileUpload)
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
            .Include(t => t.ItemMasterFileUpload)
            .Include(d => d.ItemMasterRouting)
            .Include(d => d.ItemMasterWarehouse);
            return itemmasterSADetails;
        }
        //sa,fg, and fru

        public async Task<IEnumerable<ItemMaster>> GetAllFgSaFruItems()
        {
            var itemmasterFgSaFRUDetails = FindAll().OrderByDescending(a => a.Id)
               .Where(a=>a.ItemType == PartType.SA || a.ItemType == PartType.FG || a.ItemType == PartType.FRU)
                         .Include(c => c.FileUpload)
                            .Include(x => x.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse).ToList();
            return itemmasterFgSaFRUDetails;
             

        }

        public async Task<ItemMaster> GetItemMasterById(int id)
        {
            var getItemMasterById = await TipsMasterDbContext.ItemMasters
                            .Where(x => x.Id == id)
                            .Include(c=>c.FileUpload)
                            .Include(b=>b.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                            .Include(x => x.ItemMasterApprovedVendor)
                            .Include(m => m.ItemMasterFileUpload)
                            .Include(s => s.ItemMasterRouting)
                            .Include(p => p.ItemMasterWarehouse)
                            .FirstOrDefaultAsync();


            return getItemMasterById;
        }

        public async Task<string> UpdateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.LastModifiedBy = "Admin";
            itemMaster.LastModifiedOn = DateTime.Now;
            Update(itemMaster);
            string result = $"ItemMaster details of {itemMaster.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList()
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
        public async Task<ItemMaster> GetItemMasterByItemNumber(string ItemNumber)
        {
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber)
                 .Include(c => c.FileUpload)
                .Include(x => x.ImageUpload)
                 .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)

                             .FirstOrDefaultAsync();
            return getItemMasterByItemNo;
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
         public FileUploadDocumentRepository(TipsMasterDbContext tipsMasterDbContext) : base(tipsMasterDbContext)
        {

        } 
        public async Task<int?> CreateFileUploadDocument(FileUpload fileUpload)
        {
            fileUpload.CreatedBy = "Admin";
            fileUpload.CreatedOn = DateTime.Now;
            fileUpload.LastModifiedBy = "Admin";
            fileUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(fileUpload);
            return result.Id;
        }

        public async Task<int?> CreateImageUploadDocument(FileUpload fileUpload)
        {
            fileUpload.CreatedBy = "Admin";
            fileUpload.CreatedOn = DateTime.Now;
            fileUpload.LastModifiedBy = "Admin";
            fileUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(fileUpload);
            return result.Id;
        }

    }

    public class ImageUploadDocumentRepository : RepositoryBase<ImageUpload>, IImageUploadRepository
    {
        public ImageUploadDocumentRepository(TipsMasterDbContext tipsMasterDbContext) : base(tipsMasterDbContext)
        {

        } 

        public async Task<int?> ImageUploadDocument(ImageUpload imageUpload)
        {
            imageUpload.CreatedBy = "Admin";
            imageUpload.CreatedOn = DateTime.Now;
            imageUpload.LastModifiedBy = "Admin";
            imageUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(imageUpload);
            return result.Id;
        }

    }

}
