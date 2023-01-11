using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class ForecastSourcing
    {
        [Key]
        public int Id { get; set; }
        public string? ForeCastNumber { get; set; }
        public string? CustomerName { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ForecastSourcingItems>? ForecastSourcingItems { get; set; }

    }
}
