using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class OpenSalesCoverageReport
    {
        public string? FGOrTGPartNumber { get; set; }
        public decimal? SOOpenQty { get; set; }
        public decimal? Stock { get; set; }
        public decimal? OpenPoQty { get; set; }
        public decimal? BalanceToOrder { get; set; }
        public string? Status { get; set; }
    }
}
