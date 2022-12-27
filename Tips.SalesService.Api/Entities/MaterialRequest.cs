using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class MaterialRequest
    {
        public int Id { get; set; }
        public string MRNo { get; set; }
        public string? ProjectNo { get; set; }
        public string? FGItemNo { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNo { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }
        public bool IssuedStatus { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialRequestItem>? MaterialRequestItemList { get; set; }

    }
}
