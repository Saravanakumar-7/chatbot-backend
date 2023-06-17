using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class ScheduleDate
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? Date { get; set; }

        [Precision(13, 3)]
        public decimal? Quantity { get; set; }

        public int SalesOrderItemsId { get; set; }
        public SalesOrderItems? SalesOrderItems { get; set; }
    }
}
