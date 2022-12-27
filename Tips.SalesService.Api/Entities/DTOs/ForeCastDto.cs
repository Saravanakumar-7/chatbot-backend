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
    public class ForeCastDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? RevisionNumber { get; set; }
        public int ForeCastNumber { get; set; }
        public string? CustomerForeCastRefrence { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForeCastPostDto
    {
        public string? CustomerName { get; set; }
        public string? RevisionNumber { get; set; }
        public int ForeCastNumber { get; set; }
        public string? CustomerForeCastRefrence { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForeCastUpdateDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? RevisionNumber { get; set; }
        public int ForeCastNumber { get; set; }
        public string? CustomerForeCastRefrence { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForeCastNumberListDto
    {
        public int Id { get; set; }
        public int ForeCastNumber { get; set; }
        public string? CustomerName { get; set; }
    }
}
