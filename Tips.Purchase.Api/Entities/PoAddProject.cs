using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PoAddProject
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }
        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }
        public bool PoAddProjectStatus {  get; set; }
        public int POItemDetailId { get; set; }
        public PoItem? POItemDetail { get; set; }

    }
}
