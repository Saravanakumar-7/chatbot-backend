using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;
using Org.BouncyCastle.Ocsp;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class RfqSourcingRepository : RepositoryBase<RfqSourcing>, IRfqSourcingRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqSourcingRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRfqSourcing(RfqSourcing rfqSourcing)
        {
            rfqSourcing.CreatedBy = _createdBy;
            rfqSourcing.CreatedOn = DateTime.Now;
            rfqSourcing.Unit = _unitname;            
            var result = await Create(rfqSourcing);    
            
            return result.Id;
        }

        public async Task<string> DeleteRfqSourcing(RfqSourcing rfqSourcing)
        {
            Delete(rfqSourcing);
            string result = $"rfqSourcing details of {rfqSourcing.Id} is deleted successfully!";   
             return result;
            
        }

        public async Task<PagedList<RfqSourcing>> GetAllRfqSourcing([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var getAllRfqSourcing = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue)
              || inv.RFQNumber.Contains(searchParammes.SearchValue) 
              || inv.CustomerName.Contains(searchParammes.SearchValue))))
                 .Include(t => t.RfqSourcingItems)
                 .ThenInclude(x => x.RfqSourcingVendors);

            return PagedList<RfqSourcing>.ToPagedList(getAllRfqSourcing, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<RfqSourcing> GetRfqSourcingById(int id)
        {
            var rfqSourcingById = await _tipsSalesServiceDbContext.RfqSourcings.AsNoTracking().Where(x => x.Id == id)
                               .Include(t => t.RfqSourcingItems)
                               .ThenInclude(x => x.RfqSourcingVendors)
                            .FirstOrDefaultAsync();

            return rfqSourcingById;
        }

        public async Task<RfqSourcing> GetRfqSourcingDetailsByRfqNo(string rfqNo)
        {
            var rfqSourcingByRfqNo = await _tipsSalesServiceDbContext.RfqSourcings.Where(x => x.RFQNumber == rfqNo)
                              .Include(t => t.RfqSourcingItems)
                              .ThenInclude(x => x.RfqSourcingVendors)
                           .FirstOrDefaultAsync();

            return rfqSourcingByRfqNo;
        }
        public async Task<RfqSourcingVendorDetailsDto> GetRfqSourcingVendorDetails(string ProjectNumber, string ItemNumber, string VendorId)
        {
            var rfqSourcingByRfqNo = await _tipsSalesServiceDbContext.RfqSourcings.Where(x => x.RFQNumber == ProjectNumber)
                              .Select(x=>x.Id)
                           .FirstOrDefaultAsync();

            var rfqSourcingItemByItemNo= await _tipsSalesServiceDbContext.RfqSourcingItems.Where(x => x.ItemNumber == ItemNumber
                                && x.RfqSourcingId == rfqSourcingByRfqNo)
                             .Select(x => x.Id)
                          .FirstOrDefaultAsync();

            var rfqSourcingVendorDetails = await _tipsSalesServiceDbContext.RfqSourcingVendors.Where(x => x.VendorId == VendorId
                                && x.RfqSourcingItemsId == rfqSourcingItemByItemNo)
                             .Select(x => new RfqSourcingVendorDetailsDto
                             {
                                 ProjectNumber = ProjectNumber,
                                 ItemNumber = ItemNumber,
                                 VendorId = VendorId,
                                 VendorName = x.Vendor,
                                 LandingPrice = x.LandingPrice,
                                 MoqCost = x.MoqCost

                             }).FirstOrDefaultAsync();  


            return rfqSourcingVendorDetails;
        }

        public async Task<string> UpdateRfqSourcing(RfqSourcing rfqSourcing)
        {
            rfqSourcing.LastModifiedBy = _createdBy;
            rfqSourcing.LastModifiedOn = DateTime.Now;
            Update(rfqSourcing);
            string result =$"rfqSourcing of Detail {rfqSourcing.Id} is updated successfully!";
            return result;
        }
    }

}
