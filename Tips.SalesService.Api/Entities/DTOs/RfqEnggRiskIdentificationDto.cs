using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqEnggRiskIdentificationDto
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Note { get; set; }
        
    }
    public class RfqEnggRiskIdentificationDtoPost
    {
       
        public string? Category { get; set; }

        [StringLength(500, ErrorMessage = "Note can't be longer than 500 characters")]
        public string? Note { get; set; }
       
    }
    public class RfqEnggRiskIdentificationDtoUpdate
    {
        
        public string? Category { get; set; }

        [StringLength(500, ErrorMessage = "Note can't be longer than 500 characters")]
        public string? Note { get; set; }
        
    }
}
