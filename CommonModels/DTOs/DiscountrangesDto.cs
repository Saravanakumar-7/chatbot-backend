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
        public bool IsActive { get; set; }
        public List<DiscountUsersDto> DiscountUsers { get; set; }
    }

    public class DiscountRangesPostDto
    {
        [Required(ErrorMessage = "FromAmount is required")]
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public List<DiscountUsersPostDto> DiscountUsers { get; set; }
    }

    public class DiscountRangesUpdateDto
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        [Required(ErrorMessage = "FromAmount is required")]
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public bool IsActive { get; set; }
        public List<DiscountUsersUpdateDto> DiscountUsers { get; set; }
    }
}
