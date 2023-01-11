using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tips.SalesService.Api.Entities
{
    public class MaterialTransactionNoteItem
    {
        [Key]
        public int? Id { get; set; }
        public string? FromPartNo { get; set; }
        public string? FromLocation { get; set; }
        public int? Stock { get; set; }
        public string? ToProject { get; set; }
        public string? ToPartNo { get; set; }
        public decimal? TransferQty { get; set; }
        public bool IsApproved { get; set; }
        public bool IsClosed { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int MaterialTransactionNoteId { get; set; }

        public MaterialTransactionNote? MaterialTransactionNote { get; set; }
    }
}
