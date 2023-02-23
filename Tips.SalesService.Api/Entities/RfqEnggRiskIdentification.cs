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
    public class RfqEnggRiskIdentification
    {
        [Key]
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Note { get; set; }        
        public int RfqEnggId { get; set; }
        public RfqEngg? RfqEngg { get; set; }
    }
}
