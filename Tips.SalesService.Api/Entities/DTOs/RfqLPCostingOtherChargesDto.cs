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
    public class RfqLPCostingOtherChargesDto
    {
        public int Id { get; set; }
        public string? NameOfLable { get; set; }      

        [Precision(13, 8)]
        public decimal? CostOfLable { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqLPCostingOtherChargesDtoPost
    {
        [StringLength(500, ErrorMessage = "NameOfLable can't be longer than 500 characters")]

        public string? NameOfLable { get; set; }       
        
        [Precision(13, 8)]
        public decimal? CostOfLable { get; set; }
        [Required(ErrorMessage = "Unit is required")]

        public string Unit { get; set; }
      
    }
    public class RfqLPCostingOtherChargesDtoUpdate
    {
        [StringLength(500, ErrorMessage = "NameOfLable can't be longer than 500 characters")]

        public string? NameOfLable { get; set; }      
       
        [Precision(13, 8)]
        public decimal? CostOfLable { get; set; }

        public string Unit { get; set; }


    }
}
