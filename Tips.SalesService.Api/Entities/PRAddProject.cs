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
    public class PRAddProject
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PRPurchaseId { get; set; }
        public PurchaseRequistion? PurchaseRequistion { get; set; }
    }
}
