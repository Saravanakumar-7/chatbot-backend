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
        public async Task<ForeCast>  ForeCastSourcingByForecasrNumbers(string ForecastNumber)
        {
            var SourcingByForeCastNumber = await _tipsSalesServiceDbContext.ForeCasts
              .Where(x => x.ForeCastNumber == ForecastNumber)
                        .FirstOrDefaultAsync();
            return SourcingByForeCastNumber;
        }
        public async Task<ForeCast> ForeCastLpcostingByForeCastNumbers(string ForecastNumber)
        {
            var lpcostingByForeCastNumber = await _tipsSalesServiceDbContext.ForeCasts
              .Where(x => x.ForeCastNumber == ForecastNumber)
                        .FirstOrDefaultAsync();
            return lpcostingByForeCastNumber;
        }
        public async Task<ForeCast> ForeCastLpCostingReleaseByForeCastNumbers(string ForecastNumber)
        {
            var lpcostingReleaseByForeCastNumber = await _tipsSalesServiceDbContext.ForeCasts
              .Where(x => x.ForeCastNumber == ForecastNumber)
                        .FirstOrDefaultAsync();
            return lpcostingReleaseByForeCastNumber;
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
            var getAllForeCastCS= PagedList<ForeCastCustomerSupport>.ToPagedList(FindAll()
            .Include(t => t.ForeCastCustomerSupportItems)
            .ThenInclude(u => u.ForeCastCSDeliverySchedule)
            .Include(x => x.ForeCastCustomerSupportNotes)
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return getAllForeCastCS;
        }


        public async Task<ForeCastCustomerSupport> GetForeCastCustomerSupportById(int id)
        {
            var getForeCastCSById = await _tipsSalesServiceDbContext.ForeCastCustomerSupports.Where(x => x.Id == id)
                              .Include(t => t.ForeCastCustomerSupportItems)
                              .ThenInclude(n => n.ForeCastCSDeliverySchedule)
                           .Include(m => m.ForeCastCustomerSupportNotes)
                           .FirstOrDefaultAsync();

            return getForeCastCSById;
        }



        public async Task<ForeCastCustomerSupport> ForeCastCustomerSupportByForeCastNumber(string ForeCastNumber)
        {
            var csByForeCastNumber = await _tipsSalesServiceDbContext.ForeCastCustomerSupports
                .Include(t => t.ForeCastCustomerSupportItems)
                .ThenInclude(n => n.ForeCastCSDeliverySchedule)
                .Include(m => m.ForeCastCustomerSupportNotes)
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
            var getAllForeCastEngg = PagedList<ForeCastEngg>.ToPagedList(FindAll()
            .Include(t => t.ForeCastEnggItems)
            .Include(x => x.ForeCastEnggRiskIdentifications)
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return getAllForeCastEngg;
        }

        public async Task<ForeCastEngg> GetForeCastEnggById(int id)
        {
            var GetForeCastEnggById = await _tipsSalesServiceDbContext.ForeCastEnggs.Where(x => x.Id == id)
                              .Include(t => t.ForeCastEnggItems)
                           .Include(m => m.ForeCastEnggRiskIdentifications)
                           .FirstOrDefaultAsync();

            return GetForeCastEnggById;
        }
        public async Task<ForeCastEngg> ForeCastEnggByForeCastNumber(string ForeCastNumber)
        {
            var EnggByForeCastNumber = await _tipsSalesServiceDbContext.ForeCastEnggs
                .Include(t => t.ForeCastEnggItems)
                .Include(m => m.ForeCastEnggRiskIdentifications)
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
            forecastSourcing.Unit = "Bangalore";
            var result = await Create(forecastSourcing);
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
           .Include(t => t.ForecastSourcingItems)
           .ThenInclude(x => x.ForecastSourcingVendors)
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return forecastsourcing;

        }

        public async Task<ForecastSourcing> GetForeCastSourcingById(int id)
        {
            var forecastsourcing = await _tipsSalesServiceDbContext.ForecastSourcings.Where(x => x.Id == id)
                               .Include(t => t.ForecastSourcingItems)
                               .ThenInclude(x => x.ForecastSourcingVendors)
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

    public class ForeCastCustomGroupRepository : RepositoryBase<ForeCastCustomGroup>, IForeCastCustomGroupRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public ForeCastCustomGroupRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup)
        {
            foreCastCustomGroup.CreatedBy = "Admin";
            foreCastCustomGroup.CreatedOn = DateTime.Now;
            foreCastCustomGroup.Unit = "Banglore";
            var result = await Create(foreCastCustomGroup);
            return result.Id;
        }

        public async Task<string> DeleteForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup)
        {
            Delete(foreCastCustomGroup);
            string result = $"ForeCastCustomGroup details of {foreCastCustomGroup.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ForeCastCustomGroup>> GetAllForeCastCustomGroup(PagingParameter pagingParameter)
        {
            var getAllForeCastCustomGroup = PagedList<ForeCastCustomGroup>.ToPagedList(FindAll()
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return getAllForeCastCustomGroup;
        }

        public async Task<ForeCastCustomGroup> GetForeCastCustomGroupById(int id)
        {
            var getForeCastCustomGroupById = await _tipsSalesServiceDbContext.ForeCastCustomGroups.Where(x => x.Id == id).FirstOrDefaultAsync();
            return getForeCastCustomGroupById;
        }

        public async Task<string> UpdateForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup)
        {
            foreCastCustomGroup.LastModifiedBy = "Admin";
            foreCastCustomGroup.LastModifiedOn = DateTime.Now;
            Update(foreCastCustomGroup);
            string result = $"ForeCastCustomGroup Detail {foreCastCustomGroup.Id} is updated successfully!";
            return result;
        }
    }

    public class ForeCastCustomFieldRepository : RepositoryBase<ForeCastCustomField>, IForeCastCustomFieldRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public ForeCastCustomFieldRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateForeCastCustomField(ForeCastCustomField foreCastCustomField)
        {
            foreCastCustomField.CreatedBy = "Admin";
            foreCastCustomField.CreatedOn = DateTime.Now;
            foreCastCustomField.Unit = "Banglore";
            var result = await Create(foreCastCustomField);
            return result.Id;
        }

        public async Task<string> DeleteForeCastCustomField(ForeCastCustomField foreCastCustomField)
        {
            Delete(foreCastCustomField);
            string result = $"ForeCastCustomField details of {foreCastCustomField.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ForeCastCustomField>> GetAllForeCastCustomField(PagingParameter pagingParameter)
        {
            var getAllForeCastCustomField = PagedList<ForeCastCustomField>.ToPagedList(FindAll()
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return getAllForeCastCustomField;
        }

        public async Task<ForeCastCustomField> GetForeCastCustomFieldById(int id)
        {
            var getForeCastCustomFieldById = await _tipsSalesServiceDbContext.ForeCastCustomFields.Where(x => x.Id == id).FirstOrDefaultAsync();
            return getForeCastCustomFieldById;
        }

        public async Task<string> UpdateForeCastCustomField(ForeCastCustomField foreCastCustomField)
        {
            foreCastCustomField.LastModifiedBy = "Admin";
            foreCastCustomField.LastModifiedOn = DateTime.Now;
            Update(foreCastCustomField);
            string result = $"ForeCastCustomField Detail {foreCastCustomField.Id} is updated successfully!";
            return result;
        }
    }
}