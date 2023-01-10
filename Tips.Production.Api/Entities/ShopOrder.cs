using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tips.Production.Api.Entities
{
    public class ShopOrder
    {
        [Key]      
        public int Id { get; set; }       
        public string ShopOrderNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string ProjectType { get; set; }      
        public string ItemType { get; set; }        

        [Precision(13,3)]
        public decimal? TotalSOReleaseQty { get; set; }
        public DateTime SOClosedDate { get; set; }
        public string? SAItemNumber { get; set; }       

        [Precision(13,3)]
        public decimal? CanCreateQty { get; set; }

        [Precision(13, 3)]
        public decimal? WipQty { get; set; }

        [Precision(13, 3)]
        public decimal? OqcQty { get; set; }

        [Precision(13, 3)]
        public decimal? Scrapqty { get; set; }

        [Precision(13, 3)]
        public decimal? SOReleaseQty { get; set; }
        [DefaultValue(0)]
        public OrderStatus FgDoneStatus { get; set; }
        
        public bool IsDeleted { get; set; } = false;

        [DefaultValue(0)]
        public OrderStatus Status { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }

        public string? ShortClosedBy { get; set; }

        public DateTime? ShortClosedOn { get; set; }     
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ShopOrderItem>? ShopOrderItems { get; set; }

    }
}
