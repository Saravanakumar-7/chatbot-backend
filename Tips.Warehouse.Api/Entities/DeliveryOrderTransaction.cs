using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class DeliveryOrderTransaction
    {
        [Key]
        public int Id { get; set; }

        public string? FGPartNumber { get; set; }

        public string? CustomerId { get; set; }

        public string? Description { get; set; }

        public string? CustomerName { get; set; }

        public string? DeliveryOrderNumber { get; set; }

        public string? From_form { get; set; }

        public decimal? Dispatched_Or_Return_Qty { get; set; }

        public string? Transaction_Type { get; set; }

        public string? Remark { get; set; }

        [StringLength(100), Display(Name = "Created By")]
        public string? CreatedBy { get; set; }


        [DataType(DataType.DateTime), Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }

    }
}
