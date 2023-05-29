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
using Microsoft.AspNetCore.Mvc;

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
        public async Task<PagedList<ForeCast>> GetAllForeCast([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var getAllForecastDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.ForeCastNumber.Contains(searchParammes.SearchValue) ||
                 inv.CustomerName.Contains(searchParammes.SearchValue))));

            return PagedList<ForeCast>.ToPagedList(getAllForecastDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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

        //public async Task<int?> GetForecastNumberAutoIncrementCount(DateTime date)
        //{
        //    var getForecastNumberAutoIncrementCount = _tipsSalesServiceDbContext.ForeCasts.Where(x => x.CreatedOn == date.Date).Count();

        //    return getForecastNumberAutoIncrementCount;
        //}

        //public async Task<string> GetForecastNumberAutoIncrementNumber()
        //{
        //    var getForecastNumberAutoIncrementNumber = await _tipsSalesServiceDbContext.ForeCasts.OrderByDescending(x => x.Id)
        //     .Select(x => x.ForeCastNumber)
        //     .FirstOrDefaultAsync();

        //    return getForecastNumberAutoIncrementNumber;
        //}

        public async Task<string> GenerateForecastNumber()
        {

            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var forecastNumberEntity = await _tipsSalesServiceDbContext.ForecastNos.SingleAsync();
                forecastNumberEntity.CurrentValue += 1;
                _tipsSalesServiceDbContext.Update(forecastNumberEntity);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"FC-{forecastNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<IEnumerable<ForeCastNumberListDto>> GetAllForecastNumberList()
        {
            IEnumerable<ForeCastNumberListDto> forecastNumberList = await _tipsSalesServiceDbContext.ForeCasts
                                .Select(x => new ForeCastNumberListDto()
                                {
                                   Id = x.Id,
                                    ForeCastNumber = x.ForeCastNumber,
                                    CustomerName = x.CustomerName
                                })
                              .OrderByDescending(x => x.Id).ToListAsync();

            return forecastNumberList;
        }

        public async Task<IEnumerable<ForeCastNumberListDto>> GetAllActiveForecastNumberListByCustomerId(string CustomerId)
        {
            var latestForecasts = await _tipsSalesServiceDbContext.ForeCasts
                .Where(r => r.CustomerId == CustomerId)
                .ToListAsync();

            var getAllActiveForecastNumberList = latestForecasts
                .GroupBy(r => r.ForeCastNumber)
                .SelectMany(group => group.Where(r => r.RevisionNumber == group.Max(g => g.RevisionNumber)))
                .Select(x => new ForeCastNumberListDto
                {
                    Id = x.Id,
                    ForeCastNumber = x.ForeCastNumber,
                    CustomerName = x.CustomerName,
                    CustomerId = x.CustomerId
                });

            return getAllActiveForecastNumberList;
        }

        public async Task<ForeCast> ForecastDetailsByForecastNumbers(string forecast)
        {
            var forecastDetailsByForecastNumber = await _tipsSalesServiceDbContext.ForeCasts
           .Where(x => x.ForeCastNumber == forecast)
                     .FirstOrDefaultAsync();
            return forecastDetailsByForecastNumber;
        }

        public async Task<ForeCast> GetCustomerIdByForecastNumber(string forecast)
        {
            var getCustomerId = await _tipsSalesServiceDbContext.ForeCasts
                        .Where(x => x.ForeCastNumber == forecast)
                                  .FirstOrDefaultAsync();
            return getCustomerId;
        }

        public async Task<IEnumerable<RevNumberByForecastNumberListDto>> GetRevNumberByForecastNumberList(string forecast)
        {
            IEnumerable<RevNumberByForecastNumberListDto> revNoListbyForecastNumber = await _tipsSalesServiceDbContext.ForeCasts
            .Where(x => x.ForeCastNumber == forecast).Select(x => new RevNumberByForecastNumberListDto()
            {
                RevisionNumber = x.RevisionNumber,
            }).ToListAsync();

            return revNoListbyForecastNumber; 
        }

        public async Task<ForeCast> GetForecastDeatailsByForecastNoAndRevNo(string forecast, int revisionNumber)
        {
            var forecastDetail = await _tipsSalesServiceDbContext.ForeCasts
                .Where(x => x.ForeCastNumber == forecast && x.RevisionNumber == revisionNumber)
                .FirstOrDefaultAsync();

            return forecastDetail;
        }

        public async Task<ForeCast> UpdateForecastRevNo(ForeCast forecast)
        {
            var getOldForecastDetails = _tipsSalesServiceDbContext.ForeCasts
              .Where(x => x.ForeCastNumber == forecast.ForeCastNumber && x.IsModified == false)
              .FirstOrDefault();

            if (getOldForecastDetails != null)
            {
                getOldForecastDetails.IsModified = true;
                getOldForecastDetails.LastModifiedBy = "Admin";
                getOldForecastDetails.LastModifiedOn = DateTime.Now;
                Update(getOldForecastDetails);
            }

            forecast.CreatedOn = forecast.CreatedOn;
            forecast.LastModifiedBy = "Admin";
            forecast.LastModifiedOn = DateTime.Now;
            var getOldRevisionNumber = _tipsSalesServiceDbContext.ForeCasts
                .Where(x => x.ForeCastNumber == forecast.ForeCastNumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();

            forecast.RevisionNumber = (getOldRevisionNumber + 1);
            var result = await Create(forecast);
            return result;
        }

        public async Task<ForeCast> ForeCastCustomerSupportByForeCastNumber(string ForeCastNumber)
        {
            var forecastCsByForecastNumber = await _tipsSalesServiceDbContext.ForeCasts
              .Where(x => x.ForeCastNumber == ForeCastNumber)
                        .FirstOrDefaultAsync();
            return forecastCsByForecastNumber;
        }

        public async Task<IEnumerable<LatestForecastNumberListDto>> GetAllActiveLatestForeCastNumbers()
        {
            var getAllActiveForecastNumberList = _tipsSalesServiceDbContext.ForeCasts
                .GroupBy(r => r.ForeCastNumber)
            .AsEnumerable()
            .SelectMany(group => group
            .Where(r => r.ForeCastNumber == group.Max(g => g.ForeCastNumber)))
             .Select(x => new LatestForecastNumberListDto
             {
                 ForecastNumber = x.ForeCastNumber,
                 RevisionNumber = x.RevisionNumber
             });


            return getAllActiveForecastNumberList;
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


        public async Task<PagedList<ForeCastCustomerSupport>> GetAllForeCastCustomerSupports([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {


            var getAllForeCastCS = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) ||// inv.RevisionNumber.Contains(searchParammes.SearchValue) ||
                 inv.CustomerName.Contains(searchParammes.SearchValue) || inv.ForecastNumber.Contains(searchParammes.SearchValue))))
               .Include(t => t.ForeCastCustomerSupportItems)
             .ThenInclude(u => u.ForeCastCSDeliverySchedule)
             .Include(x => x.ForeCastCustomerSupportNotes);

            return PagedList<ForeCastCustomerSupport>.ToPagedList(getAllForeCastCS, pagingParameter.PageNumber, pagingParameter.PageSize);
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



        public async Task<ForeCastCustomerSupport> GetForeCastCustomerSupportByForeCastNumber(string ForeCastNumber)
        {
            var getForecastCSByForecastNumber = await _tipsSalesServiceDbContext.ForeCastCustomerSupports
                .Where(x => x.ForecastNumber == ForeCastNumber)
                .Include(t => t.ForeCastCustomerSupportItems)
                .ThenInclude(n => n.ForeCastCSDeliverySchedule)
                .Include(m => m.ForeCastCustomerSupportNotes)
                        .FirstOrDefaultAsync();

            return getForecastCSByForecastNumber;
        }

        public async Task<string> UpdateForeCastCustomerSupport(ForeCastCustomerSupport foreCastCustomerSupport)
        {
            foreCastCustomerSupport.LastModifiedBy = "Admin";
            foreCastCustomerSupport.LastModifiedOn = DateTime.Now;
            Update(foreCastCustomerSupport);
            string result = $"ForeCast of Detail {foreCastCustomerSupport.Id} is updated successfully!";
            return result;
        }

        public async Task<ForeCastCustomerSupport> UpdateForecastcsRevNo(ForeCastCustomerSupport foreCastCustomerSupport)
        {
            foreCastCustomerSupport.CreatedBy = foreCastCustomerSupport.CreatedBy;
            foreCastCustomerSupport.CreatedOn = foreCastCustomerSupport.CreatedOn;
            foreCastCustomerSupport.LastModifiedBy = "Admin";
            foreCastCustomerSupport.LastModifiedOn = DateTime.Now;
            var getOldRevisionNumber = _tipsSalesServiceDbContext.ForeCastCustomerSupports
                .Where(x => x.ForecastNumber == foreCastCustomerSupport.ForecastNumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();

            foreCastCustomerSupport.RevisionNumber = (getOldRevisionNumber + 1);
            var result = await Create(foreCastCustomerSupport);
            return result;
        }

        public async Task<ForeCastCustomerSupport> GetForecastCsByForecastNoAndRevNo(string forecast, decimal revisionNumber)
        {
            var forecastCsByRfqNoAndRevNo = await _tipsSalesServiceDbContext.ForeCastCustomerSupports.Where(x => x.ForecastNumber == forecast
                   && x.RevisionNumber == revisionNumber)
                 .Include(x => x.ForeCastCustomerSupportItems)
                .ThenInclude(x => x.ForeCastCSDeliverySchedule)
                .Include(x => x.ForeCastCustomerSupportNotes)
               .FirstOrDefaultAsync();

            return forecastCsByRfqNoAndRevNo;
        }

        public async Task<ForeCastCustomerSupport> GetForecastCsLatestRevNoByForecastnumber(string forecast)
        {
            var forecastCsLatestRevNoByRfqNo = await _tipsSalesServiceDbContext.ForeCastCustomerSupports.Where(x => x.ForecastNumber == forecast)
                            .OrderByDescending(x => x.Id)
                             .Include(x => x.ForeCastCustomerSupportItems)
                .ThenInclude(x => x.ForeCastCSDeliverySchedule)
                .Include(x => x.ForeCastCustomerSupportNotes)
                           .FirstOrDefaultAsync();

            return forecastCsLatestRevNoByRfqNo;
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

        public async Task<IEnumerable<ForeCastCustomerSupportItem>> GetForecastCustomerSupportItemByForecastNumber(string forecastNumber)
        {
            var getForecastCSItemForecastNo = await _tipsSalesServiceDbContext.foreCastCustomerSupportItems.Where(x => x.ForecastNumber == forecastNumber).ToListAsync();
            return getForecastCSItemForecastNo;
        }

        public async Task<ForeCastCustomerSupportItem> GetForeCastCustomerSupportItemById(int id)
        {
            var getActiveStatus = await _tipsSalesServiceDbContext.foreCastCustomerSupportItems.Where(x => x.Id == id && x.ReleaseStatus == false).FirstOrDefaultAsync();
            return getActiveStatus;
        }

        public async Task<IEnumerable<ForeCastCustomerSupportItem>> GetForecastCustomerSupportRelesedDetailsByForecastNumber(string forecastNo)
        {
            var forecastCsRelesedDetails = await _tipsSalesServiceDbContext.foreCastCustomerSupportItems.Where(x => x.ForecastNumber == forecastNo
            && x.ReleaseStatus == true).ToListAsync();
            return forecastCsRelesedDetails;
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


        public async Task<PagedList<ForeCastEngg>> GetAllForeCastEngg([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {



            var getAllForeCastEngg = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.RevisionNumber.Contains(searchParammes.SearchValue) ||
                 inv.CustomerName.Contains(searchParammes.SearchValue) || inv.ForecastNumber.Contains(searchParammes.SearchValue))))
               .Include(t => t.ForeCastEnggItems)
            .Include(x => x.ForeCastEnggRiskIdentifications);

            return PagedList<ForeCastEngg>.ToPagedList(getAllForeCastEngg, pagingParameter.PageNumber, pagingParameter.PageSize);
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

        public async Task<PagedList<ForecastSourcing>> GetAllForeCastSourcing([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var forecastsourcing = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.ForeCastNumber.Contains(searchParammes.SearchValue) ||
                 inv.CustomerName.Contains(searchParammes.SearchValue))))
               .Include(t => t.ForecastSourcingItems)
           .ThenInclude(x => x.ForecastSourcingVendors);

            return PagedList<ForecastSourcing>.ToPagedList(forecastsourcing, pagingParameter.PageNumber, pagingParameter.PageSize);

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

        public async Task<ForecastLpCosting> GetForecastLpCostingByForeCastNumber(string ForeCastNumber)
        {
            var getLpCostingByForeCastNumber = await _tipsSalesServiceDbContext.ForecastLpCostings
                .Include(t => t.ForecastLpCostingItems)
              .Where(x => x.ForeCastNumber == ForeCastNumber)
                        .FirstOrDefaultAsync();
            return getLpCostingByForeCastNumber;
        }

        public async Task<PagedList<ForecastLpCosting>> GetAllForecastLpCosting([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var getAllLpCosting = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.ForeCastNumber.Contains(searchParammes.SearchValue) ||
                 inv.CustomerName.Contains(searchParammes.SearchValue))))
               .Include(x => x.ForecastLpCostingItems)
               .ThenInclude(u => u.ForecastLpCostingProcesses)
                .Include(x => x.ForecastLpCostingItems)
                .ThenInclude(v => v.ForecastLPCostingNREConsumables)
                .Include(x => x.ForecastLpCostingItems)
                .ThenInclude(w => w.ForecastLpCostingOtherCharges);

            return PagedList<ForecastLpCosting>.ToPagedList(getAllLpCosting, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<ForecastLpCosting> GetForecastLpCostingById(int id)
        {
            var getLpCostingById = await _tipsSalesServiceDbContext.ForecastLpCostings.Where(x => x.Id == id)
                 .Include(x => x.ForecastLpCostingItems)
                 .ThenInclude(u => u.ForecastLpCostingProcesses)
                 .Include(x => x.ForecastLpCostingItems)
                 .ThenInclude(v => v.ForecastLPCostingNREConsumables)
                 .Include(x => x.ForecastLpCostingItems)
                 .ThenInclude(w => w.ForecastLpCostingOtherCharges)
                          .FirstOrDefaultAsync();

            return getLpCostingById;
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


        public async Task<PagedList<ForeCastCustomGroup>> GetAllForeCastCustomGroup([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var forecastCustomGroup = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.CustomGroupName.Contains(searchParammes.SearchValue))));

            return PagedList<ForeCastCustomGroup>.ToPagedList(forecastCustomGroup, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<ListOfForecastCustomGroupDto>> GetAllForecastCustomGroupList()
        {
            IEnumerable<ListOfForecastCustomGroupDto> forecastCustomGroupLists = await _tipsSalesServiceDbContext.ForeCastCustomGroups
                              .Select(c => new ListOfForecastCustomGroupDto()
                              {
                                  Id = c.Id,
                                  CustomGroupName = c.CustomGroupName,

                              })
                              .OrderByDescending(c => c.Id)
                            .ToListAsync();

            return forecastCustomGroupLists;
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

        public async Task<PagedList<ForeCastCustomField>> GetAllForeCastCustomField([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var forecastCustomField = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.CustomGroupName.Contains(searchParammes.SearchValue)
              || inv.Type.Contains(searchParammes.SearchValue))));

            return PagedList<ForeCastCustomField>.ToPagedList(forecastCustomField, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<ForeCastCustomField>> GetForecastCustomFieldByCustomGroup(string CustomGroup)
        {
            var getForecastCustomFieldByCustomGroup = await FindByCondition(x => x.CustomGroupName == CustomGroup).ToListAsync();

            return getForecastCustomFieldByCustomGroup;
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

    public class ForeCastLPReleaseRepository : RepositoryBase<ForeCastReleaseLp>, IForeCastReleaseLpRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public ForeCastLPReleaseRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<ForeCastReleaseLp> BulkRelease(ForeCastReleaseLp foreCastReleaseLp)
        {
            foreCastReleaseLp.CreatedBy = "Admin";
            foreCastReleaseLp.CreatedOn = DateTime.Now;
            foreCastReleaseLp.Unit = "Bangalore";
            var result = await Create(foreCastReleaseLp);
            return result;
        }
    }

    public class ForeCastEnggItemRepository : RepositoryBase<ForeCastEnggItems>, IForeCastEnggItemsRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public ForeCastEnggItemRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<string> ActivateForeCastEnggItemById(ForeCastEnggItems foreCastEnggItems)
        {
            Update(foreCastEnggItems);
            string result = $"CostCenter details of {foreCastEnggItems.Id} is updated successfully!";
            return result;
        }

        public Task<int?> CreateForeCastEnggItems(ForeCastEnggItems foreCastEnggItems)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeactivateForeCastEnggItemById(ForeCastEnggItems foreCastEnggItems)
        {
         
            Update(foreCastEnggItems);
            string result = $"CostCenter details of {foreCastEnggItems.Id} is updated successfully!";
            return result;
        }

        public Task<string> DeleteForeCastEnggItems(ForeCastEnggItems foreCastEnggItems)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ForeCastEnggItems>> GetAllActiveForeCastEnggItemByForeCastNumber(string forecastNumber)
        {
            int poId = await _tipsSalesServiceDbContext.ForeCastEnggs
              .Where(s => s.ForecastNumber == forecastNumber).Select(x => x.Id).FirstOrDefaultAsync();

            IEnumerable<ForeCastEnggItems> foreCastEnggItems = await _tipsSalesServiceDbContext.ForeCastEnggItems
                 .Where(x => x.ForeCastEnggId == poId)
             .Where(x => x.ReleaseStatus == true).ToListAsync();

            return foreCastEnggItems;
        }

        public Task<IEnumerable<ForeCastEnggItems>> GetAllForeCastEnggItems()
        {
            throw new NotImplementedException();
        }

        public async Task<ForeCastEnggItems> GetForeCastEnggItemById(int id)
        {
            var foreCastEnggItems = await _tipsSalesServiceDbContext.ForeCastEnggItems.Where(x => x.Id == id).FirstOrDefaultAsync();
            return foreCastEnggItems;
        }

        public Task<ForeCastEnggItems> GetForeCastEnggItemsById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateForeCastEnggItems(ForeCastEnggItems foreCastEnggItems)
        {
            throw new NotImplementedException();
        }
    }

}