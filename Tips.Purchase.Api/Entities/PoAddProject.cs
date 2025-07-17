using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities
{
    public class PoAddProject
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public decimal BalanceQty { get; set; }
        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }
        public PoStatus PoAddProjectStatus {  get; set; }
        public int POItemDetailId { get; set; }
        public PoItem? POItemDetail { get; set; }
        public List<PoAddKitProject>? PoAddKitProjects { get; set; }

    }
}
