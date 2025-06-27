using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqLPCostingDto
    {
        public int Id { get; set; }
        public string? RfqNumber { get; set; }
        public string? CustomerName { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqLPCostingItemDto>? RfqLPCostingItems { get; set; }
        public decimal? RevisionNumber { get; set; }

    }
    public class RfqLPCostingDtoPost
    {
        
        public string? RfqNumber { get; set; }       
        public decimal? RevisionNumber { get; set; }
        public string? CustomerName { get; set; }

        public List<RfqLPCostingItemDtoPost>? RfqLPCostingItems { get; set; }

    }
    public class RfqLPCostingDtoUpdate
    {
        public int Id { get; set; }
        public string? RfqNumber { get; set; }
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]

        public string? CustomerName { get; set; }
        public string Unit { get; set; }
        public List<RfqLPCostingItemDtoUpdate>? RfqLPCostingItems { get; set; }

    }
}
