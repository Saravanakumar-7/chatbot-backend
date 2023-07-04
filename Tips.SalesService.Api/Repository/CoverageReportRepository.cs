using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class CoverageReportRepository : RepositoryBase<CoverageReport>, ICoverageReportRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public CoverageReportRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<List<CoverageReport>> GetAllCollectionTrackers()
        {
            var result = _tipsSalesServiceDbContext.SalesOrders
                .GroupJoin(
                    _tipsSalesServiceDbContext.SalesOrdersItems,
                    s => s.Id,
                    si => si.SalesOrderId,
                    (s, items) => new { SalesOrder = s, Items = items.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.Items,
                    (x, i) => new { x.SalesOrder, Item = i }
                )
                .Where(x => x.Item.BalanceQty > 0)
                .GroupBy(
                    x => new { x.Item.ItemNumber, x.Item.Description },
                    x => x.Item.BalanceQty,
                    (key, balances) => new
                    {
                        ItemNumber = key.ItemNumber,
                        Description = key.Description,
                        BalanceForcastQty = balances.Sum()
                    }
                )
                .ToListAsync();

            return result; 
        }

    }
} 