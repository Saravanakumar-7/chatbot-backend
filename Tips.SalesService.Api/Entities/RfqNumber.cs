using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities

{
    public class RfqNumber
    {
        [Key]
        public int RfqNO { get; set; } = 0;      

    }
}
