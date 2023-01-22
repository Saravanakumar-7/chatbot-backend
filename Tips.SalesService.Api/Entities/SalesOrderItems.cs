using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class SalesOrderItems
    {
        [Key]
        public int Id { get; set; } 
        public string? SAItemNo { get; set; }
        public string? Description { get; set; }
        public string? SalesOrderNumber { get; set; }

        public string? ProjectNumber { get; set; }
        public bool? SOPartStatus { get; set; } = false;
        public decimal? BalanceQunatity { get; set; }
        public decimal? DispachedQunatity { get; set; }
        public decimal? ShopOrderQunatity { get; set; }
        public decimal UOM { get; set; }
        public decimal Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remarks { get; set; }       
        public int SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }
    }
}
