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
    public class ForecastSourcingDto
    {
        public int Id { get; set; }
        public string? ForeCastNumber { get; set; }
        public string? CustomerName { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ForecastSourcingItemsDto>? forecastSourcingItems { get; set; }
       

    }
    public class ForecastSourcingDtoPost
    {
        [StringLength(500, ErrorMessage = "ForeCastNumber can't be longer than 100 characters")]

        public string? ForeCastNumber { get; set; }
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]

        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
        public List<ForecastSourcingItemsDtoPost>? forecastSourcingItems { get; set; }
       


    }
    public class ForecastSourcingDtoUpdate
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessage = "ForeCastNumber can't be longer than 100 characters")]

        public string? ForeCastNumber { get; set; }
        [StringLength(500, ErrorMessage = "CustomerName can't be longer than 500 characters")]

        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
        public List<ForecastSourcingItemsDtoUpdate>? forecastSourcingItems { get; set; }
        

    }
}
