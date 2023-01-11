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
    public class ForeCastEnggItems
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerItemNumber { get; set; }
        public string Description { get; set; }
        [Precision(13, 8)]
        public decimal? Qty { get; set; }
        public bool ReleaseStatus { get; set; } = false;
        public string? CostingBomVersionNumber { get; }
        public string? ItemNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int ForeCastEnggId { get; set; }
        public ForeCastEngg? ForeCastEngg { get; set; }
    }
}
