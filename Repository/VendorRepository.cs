using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class VendorRepository : RepositoryBase<VendorMaster>, IVendorRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public VendorRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateVendorMaster(VendorMaster vendorMaster)
        {
                vendorMaster.CreatedBy = _createdBy;
                vendorMaster.CreatedOn = DateTime.Now;
                vendorMaster.LastModifiedBy = _createdBy;
                vendorMaster.LastModifiedOn = DateTime.Now;
                 vendorMaster.Unit = _unitname;
                 var result = await Create(vendorMaster);
           
            return result.Id; 

        }

        public async Task<string> GenerateVendorId()
        {
            using var transaction = await TipsMasterDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await TipsMasterDbContext.VendorIds.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                TipsMasterDbContext.Update(poNumberEntity);
                await TipsMasterDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"VD-{poNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<IEnumerable<VendorMaster>> GetAllActiveVendorMasters()
        {
            var getAllActiveVendorMastersList = await FindAll().OrderByDescending(x => x.Id).ToListAsync();
            return getAllActiveVendorMastersList;
            

        }
        public async Task<PagedList<VendorMaster>> GetAllVendorMasters(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var itemmasterDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue)
            || inv.VendorType.Contains(searchParams.SearchValue) || inv.VendorAliasName.Contains(searchParams.SearchValue)
            || inv.PurchaseGroup.Contains(searchParams.SearchValue) || inv.TypeOfCompany.Contains(searchParams.SearchValue))))
            .Include(a => a.VendorBankings)
            .Include(a => a.Addresses)
            .Include(a => a.RelatedVendors)
            .Include(a => a.HeadCountings)
            .Include(a => a.Contacts);

            return PagedList<VendorMaster>.ToPagedList(itemmasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        //public async Task<PagedList<VendorMaster>> GetAllVendorMasters(PagingParameter pagingParameter, SearchParames searchParams)
        //{
        //    var itemmasterDetails = FindAll().OrderByDescending(x => x.Id)
        //    .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue) ||
        //    inv.VendorType.Contains(searchParams.SearchValue) || inv.VendorAliasName.Contains(searchParams.SearchValue))))
        //    .Include(a => a.VendorBankings)
        //    .Include(a => a.Addresses)
        //    .Include(a => a.RelatedVendors)
        //    .Include(a => a.HeadCountings)
        //    .Include(a => a.Contacts);

        //    return PagedList<VendorMaster>.ToPagedList(itemmasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<VendorMaster> GetVendorMasterById(int id)
        {
          var getVendorMasterbyId = await TipsMasterDbContext.VendorMasters.Where(x=> x.Id == id)
                              .Include(x => x.VendorBankings)
                              .Include(x => x.Addresses)
                              .Include(a => a.RelatedVendors)
                              .Include(m => m.Contacts)
                              .Include(v => v.HeadCountings)
                              .FirstOrDefaultAsync();

          return getVendorMasterbyId;
          
        }
         

        public async Task<string> UpdateVendorMaster(VendorMaster DataUpdate)             
        {
            DataUpdate.LastModifiedBy = _createdBy;
            DataUpdate.LastModifiedOn = DateTime.Now; 
            Update(DataUpdate);
            string result = $"VendorMaster of Detail {DataUpdate.Id} is updated successfully!";
            return result; 
        }

        public async Task<string> DeleteVendorMaster(VendorMaster vendormaster)
        {
            Delete(vendormaster);
            string result = $"VendorMaster details of {vendormaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<VendorIdNameListDto>> GetAllActiveVendorMasterNameList()
        {
            IEnumerable<VendorIdNameListDto> getAllActiveVendorMasterNameList = await TipsMasterDbContext.VendorMasters
                                .Where(x => x.IsActive == true)
                                .Select(x => new VendorIdNameListDto()
                                {
                                    Id = x.Id,
                                    VednorAliasName = x.VendorAliasName,
                                    VendorName = x.VendorName,
                                    VendorId = x.VendorId,
                                    VendorCategory = x.VendorCategory,
                                    VendorType = x.VendorType,
                                })
                              .ToListAsync();

            return getAllActiveVendorMasterNameList;
        }

        public async Task<IEnumerable<VendorIdNameListDto>> GetAllVendorMasterNameList()
        {
            IEnumerable<VendorIdNameListDto> getAllActiveVendorMasterNameList = await TipsMasterDbContext.VendorMasters

                                .Select(x => new VendorIdNameListDto()
                                {
                                    Id = x.Id,
                                    VednorAliasName = x.VendorAliasName,
                                    VendorName = x.VendorName,
                                    VendorId = x.VendorId,
                                    VendorCategory = x.VendorCategory,
                                    VendorType = x.VendorType,
                                })
                              .ToListAsync();

            return getAllActiveVendorMasterNameList;
        }

    }
}
