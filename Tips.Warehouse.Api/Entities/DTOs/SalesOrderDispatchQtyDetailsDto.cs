using System.ComponentModel;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class SalesOrderDispatchQtyDetailsDto
    { 
            public string FGItemNumber { get; set; }
            public int SalesOrderId { get; set; }
            public decimal DispatchQty { get; set; } 
    }
}
