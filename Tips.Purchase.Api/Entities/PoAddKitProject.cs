using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities
{
    public class PoAddKitProject
    {
        [Key]
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public decimal BalanceQty { get; set; }
        [Precision(13, 3)]
        public decimal ReceivedQty { get; set; }
        public bool PoAddKitProjectStatus { get; set; }
        public int PoAddProjectId { get; set; }
        public PoAddProject? PoAddProject { get; set; }
    }
}
