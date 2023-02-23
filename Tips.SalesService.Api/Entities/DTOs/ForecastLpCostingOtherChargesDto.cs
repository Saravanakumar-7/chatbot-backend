using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ForecastLpCostingOtherChargesDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [Precision(13, 8)]
        public decimal? Value { get; set; }       
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForecastLPCostingOtherChargesDtoPost
    {
        [StringLength(500, ErrorMessage = "NameOfLable can't be longer than 500 characters")]

        public string? Name { get; set; }

        [Precision(13, 8)]
        public decimal? Value { get; set; }
       

    }
    public class ForecastLPCostingOtherChargesDtoUpdate
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "NameOfLable can't be longer than 500 characters")]

        public string? Name { get; set; }

        [Precision(13, 8)]
        public decimal? Value { get; set; }       

    }
}
