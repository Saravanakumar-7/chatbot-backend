using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ScheduleDateDto
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? Date { get; set; }

        [Precision(13, 1)]
        public decimal? Quantity { get; set; }
    }
    public class ScheduleDatePostDto
    {
        public string? ItemNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? Date { get; set; }

        [Precision(13, 1)]
        public decimal? Quantity { get; set; }
    }
    public class ScheduleDateUpdateDto
    {
        public string? ItemNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? Date { get; set; }

        [Precision(13, 1)]
        public decimal? Quantity { get; set; }
    }
}
