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
using Tips.SalesService.Api.Entities.DTOs;
using System.Collections.Immutable;
using Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class ForeCastRepository : RepositoryBase<ForeCast>, IForeCastRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public ForeCastRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateForeCast(ForeCast foreCast)
        {
            foreCast.CreatedBy = "Admin";
            foreCast.CreatedOn = DateTime.Now;
            foreCast.Unit = "Bangalore";
            var result = await Create(foreCast);
            return result.Id;
        }

        public async Task<string> DeleteForeCast(ForeCast foreCast)
        {
            Delete(foreCast);
            string result = $"ForeCast details of {foreCast.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ForeCastNumberListDto>> GetAllActiveForeCastNumberList()
        {
            IEnumerable<ForeCastNumberListDto> forecastlistDetails = await _tipsSalesServiceDbContext.ForeCasts
                                .Select(x => new ForeCastNumberListDto()
                                {
                                    Id = x.Id,
                                    ForeCastNumber = x.ForeCastNumber,
                                    CustomerName = x.CustomerName
                                })
                              .ToListAsync();

            return forecastlistDetails;
        }

        public async Task<PagedList<ForeCast>> GetAllForeCast(PagingParameter pagingParameter)
        {
            var forecast = PagedList<ForeCast>.ToPagedList(FindAll()
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return forecast;
        }

        public async Task<ForeCast> GetForeCastById(int id)
        {
            var forecast = await _tipsSalesServiceDbContext.ForeCasts.Where(x => x.Id == id)
                          .FirstOrDefaultAsync();

            return forecast;
        }

        public async Task<string> UpdateForeCast(ForeCast foreCast)
        {
            foreCast.LastModifiedBy = "Admin";
            foreCast.LastModifiedOn = DateTime.Now;
            Update(foreCast);
            string result = $"ForeCast of Detail {foreCast.Id} is updated successfully!";
            return result;
        }
    }
    public class ForeCastCustomerSupportRepository : RepositoryBase<ForeCastCustomerSupport>, IForeCastCustomerSupportRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public ForeCastCustomerSupportRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport)
        {
            foreCastCustomerSupport.CreatedBy = "Admin";
            foreCastCustomerSupport.CreatedOn = DateTime.Now;
            foreCastCustomerSupport.Unit = "Bangalore";
            var result = await Create(foreCastCustomerSupport);
            return result.Id;
        }



        public async Task<string> DeleteForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport)
        {
            Delete(foreCastCustomerSupport);
            string result = $"ForeCastCustomerSupport details of {foreCastCustomerSupport.Id} is deleted successfully!";
            return result;
        }


        public async Task<PagedList<ForeCastCustomerSupport>> GetAllForeCastCustomerSupports(PagingParameter pagingParameter)
        {
            var foreCastCustomerSupport = PagedList<ForeCastCustomerSupport>.ToPagedList(FindAll()
            .Include(t => t.foreCastCustomerSupportItems)
            .ThenInclude(u => u.foreCastCSDeliverySchedule)
            .Include(x => x.foreCastCustomerSupportNotes)
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return foreCastCustomerSupport;
        }


        public async Task<ForeCastCustomerSupport> GetForeCastCustomerSupportById(int id)
        {
            var forecastCustomerSupport = await _tipsSalesServiceDbContext.ForeCastCustomerSupports.Where(x => x.Id == id)
                              .Include(t => t.foreCastCustomerSupportItems)
                              .ThenInclude(n => n.foreCastCSDeliverySchedule)
                           .Include(m => m.foreCastCustomerSupportNotes)
                           .FirstOrDefaultAsync();

            return forecastCustomerSupport;
        }

       

        public async Task<ForeCastCustomerSupport> ForeCastCustomerSupportByForeCastNumber(string ForeCastNumber)
        {
            var csByForeCastNumber = await _tipsSalesServiceDbContext.ForeCastCustomerSupports
                .Include(t => t.foreCastCustomerSupportItems)
                .ThenInclude(n => n.foreCastCSDeliverySchedule)
                .Include(m => m.foreCastCustomerSupportNotes)
              .Where(x => x.ForecastNumber == ForeCastNumber)
                        .FirstOrDefaultAsync();
            return csByForeCastNumber;
        }

        public async Task<string> UpdateForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport)
        {
            foreCastCustomerSupport.LastModifiedBy = "Admin";
            foreCastCustomerSupport.LastModifiedOn = DateTime.Now;
            Update(foreCastCustomerSupport);
            string result = $"ForeCast of Detail {foreCastCustomerSupport.Id} is updated successfully!";
            return result;
        }


    }
    public class ForeCastCustomerSupportItemsRepository : RepositoryBase<ForeCastCustomerSupportItem>, IForeCastCustomerSupportItemRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public ForeCastCustomerSupportItemsRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }      

        public async Task<string> ActivateForeCastCustomerSupportItemById(ForeCastCustomerSupportItem foreCastCustomerSupportItem)
        {
            foreCastCustomerSupportItem.LastModifiedBy = "Admin";
            foreCastCustomerSupportItem.LastModifiedOn = DateTime.Now;
            Update(foreCastCustomerSupportItem);
            string result = $"CostCenter details of {foreCastCustomerSupportItem.Id} is updated successfully!";
            return result;
        }

        public Task<int?> CreateForeCastCustomerSupportItem(ForeCastCustomerSupportItem foreCastCustomerSupportItem)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteForeCastCustomerSupportItem(ForeCastCustomerSupportItem foreCastCustomerSupportItem)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ForeCastCustomerSupportItem>> GetAllForeCastCustomerSupportItem()
        {
            throw new NotImplementedException();
        }

        public async Task<ForeCastCustomerSupportItem> GetForeCastCustomerSupportItemById(int id)
        {
            var getActiveStatus = await _tipsSalesServiceDbContext.foreCastCustomerSupportItems.Where(x => x.Id == id && x.ReleaseStatus == false).FirstOrDefaultAsync();
            return getActiveStatus;
        }

        public Task<string> UpdateForeCastCustomerSupportItem(ForeCastCustomerSupportItem foreCastCustomerSupportItem)
        {
            throw new NotImplementedException();
        }

    }

    //ForeCastEngg
    public class ForeCastEnggRepository : RepositoryBase<ForeCastEngg>, IForeCastEnggRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public ForeCastEnggRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateForeCastEngg(ForeCastEngg foreCastEngg)
        {
            foreCastEngg.CreatedBy = "Admin";
            foreCastEngg.CreatedOn = DateTime.Now;
            foreCastEngg.Unit = "Bangalore";
            var result = await Create(foreCastEngg);
            return result.Id;
        }

        public async Task<string> DeleteForeCastEngg(ForeCastEngg foreCastEngg)
        {
            Delete(foreCastEngg);
            string result = $"ForeCastEngg details of {foreCastEngg.Id} is deleted successfully!";
            return result;
        }


        public async Task<PagedList<ForeCastEngg>> GetAllForeCastEngg(PagingParameter pagingParameter)
        {
            var foreCastEngg = PagedList<ForeCastEngg>.ToPagedList(FindAll()
            .Include(t => t.foreCastEnggItems)
            .Include(x => x.foreCastEnggRiskIdentifications)
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return foreCastEngg;
        }

        public async Task<ForeCastEngg> GetForeCastEnggById(int id)
        {
            var foreCastEngg = await _tipsSalesServiceDbContext.ForeCastEnggs.Where(x => x.Id == id)
                              .Include(t => t.foreCastEnggItems)
                           .Include(m => m.foreCastEnggRiskIdentifications)
                           .FirstOrDefaultAsync();

            return foreCastEngg;
        }
        public async Task<ForeCastEngg> ForeCastEnggByForeCastNumber(string ForeCastNumber)
        {
            var EnggByForeCastNumber = await _tipsSalesServiceDbContext.ForeCastEnggs
                .Include(t => t.foreCastEnggItems)
                .Include(m => m.foreCastEnggRiskIdentifications)
              .Where(x => x.ForecastNumber == ForeCastNumber)
                        .FirstOrDefaultAsync();
            return EnggByForeCastNumber;
        }

        public async Task<string> UpdateForeCastEngg(ForeCastEngg foreCastEngg)
        {
            foreCastEngg.LastModifiedBy = "Admin";
            foreCastEngg.LastModifiedOn = DateTime.Now;
            Update(foreCastEngg);
            string result = $"ForeCastEngg of Detail {foreCastEngg.Id} is updated successfully!";
            return result;
        }
    }

    //Forecast Sourcing
    public class ForeCastSourcingRepository : RepositoryBase<ForecastSourcing>, IForecastSourcingRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public ForeCastSourcingRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }

        public async Task<int?> CreateForeCastSourcing(ForecastSourcing forecastSourcing)
        {
            forecastSourcing.CreatedBy = "Admin";
            forecastSourcing.CreatedOn = DateTime.Now;
            var result = await Create(forecastSourcing);
            forecastSourcing.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteForeCastSourcing(ForecastSourcing forecastSourcing)
        {
            Delete(forecastSourcing);
            string result = $"ForeCastSourcing details of {forecastSourcing.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ForecastSourcing>> GetAllForeCastSourcing(PagingParameter pagingParameter)
        {
            var forecastsourcing = PagedList<ForecastSourcing>.ToPagedList(FindAll()
           .Include(t => t.forecastSourcingItems)
           .ThenInclude(x => x.forecastSourcingVendors)
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return forecastsourcing;

        }

        public async Task<ForecastSourcing> GetForeCastSourcingById(int id)
        {
            var forecastsourcing = await _tipsSalesServiceDbContext.ForecastSourcings.Where(x => x.Id == id)
                               .Include(t => t.forecastSourcingItems)
                               .ThenInclude(x => x.forecastSourcingVendors)
                            .FirstOrDefaultAsync();

            return forecastsourcing;
        }

        public async Task<string> UpdateForeCastSourcing(ForecastSourcing forecastSourcing)
        {
            forecastSourcing.LastModifiedBy = "Admin";
            forecastSourcing.LastModifiedOn = DateTime.Now;
            Update(forecastSourcing);
            string result = $"ForeCastSourcing of Detail {forecastSourcing.Id} is updated successfully!";
            return result;
        }
    }
    // forecast Lpcosting repository 
    public class ForecastLpCostingRepository : RepositoryBase<ForecastLpCosting>, IForecastLpCostingRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public ForecastLpCostingRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateForecastLpCosting(ForecastLpCosting forecastLpCosting)
        {
            forecastLpCosting.CreatedBy = "Admin";
            forecastLpCosting.CreatedOn = DateTime.Now;
            forecastLpCosting.Unit = "Bangalore";
            var result = await Create(forecastLpCosting);
            return result.Id;
        }

        public async Task<string> DeleteForecastLpCosting(ForecastLpCosting forecastLpCosting)
        {
            Delete(forecastLpCosting);
            string result = $"forecastLpCosting details of {forecastLpCosting.Id} is deleted successfully!";
            return result;
        }

        public async Task<ForecastLpCosting> ForecastLpCostingByForeCastNumber(string ForeCastNumber)
        {
            var LpCostingByForeCastNumber = await _tipsSalesServiceDbContext.ForecastLpCostings
                .Include(t => t.forecastLpCostingItems)
              .Where(x => x.ForeCastNumber == ForeCastNumber)
                        .FirstOrDefaultAsync();
            return LpCostingByForeCastNumber;
        }

        public async Task<PagedList<ForecastLpCosting>> GetAllForecastLpCosting(PagingParameter pagingParameter)
        {
            var forecastlpCosting = PagedList<ForecastLpCosting>.ToPagedList(FindAll()
               .Include(x => x.forecastLpCostingItems)
               .ThenInclude(u => u.forecastLpCostingProcesses)
                .Include(x => x.forecastLpCostingItems)
                .ThenInclude(v => v.forecastLPCostingNREConsumables)
                .Include(x => x.forecastLpCostingItems)
                .ThenInclude(w => w.forecastLpCostingOtherCharges)
          .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return forecastlpCosting;
        }

        public async Task<ForecastLpCosting> GetForecastLpCostingById(int id)
        {
            var forecastLPCosting = await _tipsSalesServiceDbContext.ForecastLpCostings.Where(x => x.Id == id)
                 .Include(x => x.forecastLpCostingItems)
                 .ThenInclude(u => u.forecastLpCostingProcesses)
                 .Include(x => x.forecastLpCostingItems)
                 .ThenInclude(v => v.forecastLPCostingNREConsumables)
                 .Include(x => x.forecastLpCostingItems)
                 .ThenInclude(w => w.forecastLpCostingOtherCharges)
                          .FirstOrDefaultAsync();

            return forecastLPCosting;
        }

        public async Task<string> UpdateForecastLpCosting(ForecastLpCosting forecastLpCosting)
        {
            forecastLpCosting.LastModifiedBy = "Admin";
            forecastLpCosting.LastModifiedOn = DateTime.Now;
            Update(forecastLpCosting);
            string result = $"forecastLpCosting of Detail {forecastLpCosting.Id} is updated successfully!";
            return result;
        }
    }
}
