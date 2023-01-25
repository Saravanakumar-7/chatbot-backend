using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class MaterialRequestDto
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }
        public bool IssuedStatus { get; set; }

        [DefaultValue(0)]
        public ShortStatus StatusOfShort { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialRequestItemDto>? MaterialRequestItemDtos { get; set; }
    }

    public class MaterialRequestPostDto
    {


      
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }
        public bool IssuedStatus { get; set; }


        public List<MaterialRequestItemPostDto>? MaterialRequestItemPostDtos { get; set; }
    }

    public class MaterialRequestUpdateDto
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }
        public bool IssuedStatus { get; set; }    


        public List<MaterialRequestItemUpdateDto>? MaterialRequestItemUpdateDtos { get; set; }
    }

    public class MaterialRequestIdNoDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
    }
}
