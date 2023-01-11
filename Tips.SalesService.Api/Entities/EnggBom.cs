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
    public class EnggBom
    {
        [Key]
        public int Id { get; set; }

        public string ItemNumber { get; set; }

        public string ItemDescription { get; set; }

        public string  ItemType { get; set; }

        public string RevisionNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
