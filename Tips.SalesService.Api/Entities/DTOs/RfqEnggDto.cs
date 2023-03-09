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
    public class RfqEnggDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }

        public string? CustomerAliaseName { get; set; }
         

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public string? RfqNumber { get; set; }
        public string? CustomerRfqNumber { get; set; }
 
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<RfqEnggItemDto>? RfqEnggItems { get; set; }
        public List<RfqEnggRiskIdentificationDto>? RfqEnggRiskIdentifications { get; set; }

    }
    public class RfqEnggDtoPost
    {
        public string? CustomerName { get; set; }
        public string? CustomerAliaseName { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public string? RfqNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }       
        public List<RfqEnggItemDtoPost>? RfqEnggItems { get; set; }

        public List<RfqEnggRiskIdentificationDtoPost>? RfqEnggRiskIdentifications { get; set; }


    }
    public class RfqEnggDtoUpdate
    {
        public int Id { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerAliaseName { get; set; }



        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public string? RfqNumber { get; set; }

        [StringLength(500, ErrorMessage = "CustomerRfqNumber can't be longer than 500 characters")]
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }      
        public List<RfqEnggItemDtoUpdate>? RfqEnggItems { get; set; }
        public List<RfqEnggRiskIdentificationDtoUpdate>? RfqEnggRiskIdentifications { get; set; }

    }
}
