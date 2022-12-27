using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Entities
{
    public class MaterialTransactionNote
    {
        public int? Id { get; set; }
        public string? MTNNO { get; set; }
        public string? ProjectNo { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialTransactionNoteItem>? MaterialTransactionNoteItems { get; set; }
    }
}