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
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }
        public Enum IssuedStatus { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialRequestItem>? MaterialRequestItems { get; set; }

    }
}
