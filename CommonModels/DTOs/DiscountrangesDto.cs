using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class DiscountRangesDto
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        [Required(ErrorMessage = "FromAmount is required")]
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }

        [Required(ErrorMessage = "IsSpecialDiscountAllowed is required")]
        public bool IsSpecialDiscountAllowed { get; set; }
    }

    public class DiscountRangesPostDto
    {
        [Required(ErrorMessage = "FromAmount is required")]
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        [Required(ErrorMessage = "IsSpecialDiscountAllowed is required")]
        public bool IsSpecialDiscountAllowed { get; set; }
    }

    public class DiscountRangesUpdateDto
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        [Required(ErrorMessage = "FromAmount is required")]
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }

        [Required(ErrorMessage = "IsSpecialDiscountAllowed is required")]
        public bool IsSpecialDiscountAllowed { get; set; }
    }
}
