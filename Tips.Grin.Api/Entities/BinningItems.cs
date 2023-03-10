using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities
{
    public class BinningItems
    {
        [Key]
        public int Id { get; set; }
        public string? ItemNumber { get; set; } 
         
        public int GrinPartId { get; set; } 
        
        public int BinningId { get; set; }
        public Binning? Binning { get; set; }

        public List<BinningLocation>? BinningLocations { get; set; }
    }
}