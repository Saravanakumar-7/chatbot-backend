using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using System.Collections.Generic;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities.Enum;

namespace Tips.SalesService.Api.Repository
{
    public class CoverageReportRepository : RepositoryBase<CoverageReport>, ICoverageReportRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public CoverageReportRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }
        public async Task<List<SalesOrder>> GetAllForecastSalesOrderDetails()
        {
            var salesOrderDetails = await _tipsSalesServiceDbContext.SalesOrders
               .Where(x => x.SalesOrderStatus == SalesOrderStatus.Forecast)
               .ToListAsync();

            return salesOrderDetails;
        }
        public async Task<List<SalesOrderItems>> GetAllSalesOrderItemDetails(int salesorderId)
        {
            var salesOrderItemDetails = await _tipsSalesServiceDbContext.SalesOrdersItems
               .Where(x => x.SalesOrderId == salesorderId && x.StatusEnum != OrderStatus.Closed)
               .ToListAsync();

            return salesOrderItemDetails;
        }

        public async Task<IEnumerable<CoverageReportDto>> GetAllSalesOrderDetails()
        {
            List<CoverageReportDto> result = await (from k in (
                from m in (
                    from so in _tipsSalesServiceDbContext.SalesOrders
                    select new { so.SalesOrderNumber, so.ProjectNumber }
                )
                join n in (
                    from soi in _tipsSalesServiceDbContext.SalesOrdersItems
                    where new[] { OrderStatus.Open, OrderStatus.PartiallyClosed }.Contains(soi.StatusEnum)
                    select new { soi.ItemNumber, soi.SalesOrderNumber, soi.Description, soi.BalanceQty }
                ) on m.SalesOrderNumber equals n.SalesOrderNumber into nj
                from n in nj.DefaultIfEmpty()
                select new { n.ItemNumber, n.Description, n.BalanceQty }
            )
                              group k by new { k.ItemNumber, k.Description } into g
                              select new CoverageReportDto
                              {
                                  ItemNumber = g.Key.ItemNumber,
                                  ItemDescription =g.Key.Description,
                                  BalanceForcastQty = g.Sum(k => k.BalanceQty)                                   
                              }).ToListAsync();


            //= await _tipsSalesServiceDbContext.SalesOrders
            //   .GroupJoin(
            //       _tipsSalesServiceDbContext.SalesOrdersItems,
            //       s => s.Id,
            //       si => si.SalesOrderId,
            //       (s, items) => new { SalesOrder = s, Items = items.DefaultIfEmpty() }
            //   )
            //   .SelectMany(
            //       x => x.Items,
            //       (x, i) => new { x.SalesOrder, Item = i }
            //   )
            //   .Where(x => x.Item.BalanceQty > 0)
            //   .GroupBy(
            //       x => new { x.Item.ItemNumber, x.Item.Description },
            //       x => x.Item.BalanceQty,
            //       (key, balances) => new CoverageReportDto
            //       {
            //           ItemNumber = key.ItemNumber,
            //           Description = key.Description,
            //           BalanceForcastQty = balances.Sum()
            //       }
            //   )
            //   .ToListAsync();

            return result; 
        }

    }
} 