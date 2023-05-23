using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class BTODeliveryOrderDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }

        [Precision(18, 3)]
        public decimal? SOTotal { get; set; }
        public string? Remarks { get; set; }
        public string? CustomerId { get; set; }
        public string? BTONumber { get; set; }       
        public int? SalesOrderId { get; set; }
        public string SalesOrderNumber { get; set; }

        [Precision(13, 1)]
        public int? SalesOrderRevisionNumber { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? DODate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public decimal? TotalValue { get; set; }
        public string? OrderType { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItemsDto>? bTODeliveryOrderItems { get; set; }
    }
    public class BTODeliveryOrderDtoPost
    {

        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? Remarks { get; set; }
        public int? SalesOrderId { get; set; }
        public string SalesOrderNumber { get; set; }

        [Precision(13, 1)]
        public int? SalesOrderRevisionNumber { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? DODate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public decimal? TotalValue { get; set; }
        public string? OrderType { get; set; }
        public List<BTODeliveryOrderItemsDtoPost>? BTODeliveryOrderItemsDtoPost { get; set; }
    }
    public class BTODeliveryOrderDtoUpdate
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }

        [Precision(13, 1)]
        public int? SalesOrderRevisionNumber { get; set; }
        public string? Remarks { get; set; }
        public string? BTONumber { get; set; }
        public string? CustomerId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? DODate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public decimal? TotalValue { get; set; }
        public string? OrderType { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItemsDtoUpdate>? BTODeliveryOrderItemsDtoUpdate { get; set; }

    }
    public class ListofBtoDeliveryOrderDetails
    {
        public int? BtoDeliveryOrderId { get; set; }
        public string? BTONumber { get; set; }
    }

    public class ListOfBtoNumberDetails
    {
        public string? CustomerLeadID { get; set; }
        public string? BTONumber { get; set; }
        public int? BtoDeliveryOrderId { get ; set; }
        public string? OrderType { get; set; }
        public decimal? TotalValue { get; set; }
    }
    public class BtoIDNameList
    {
        public int Id { get; set; }
        public string? BTONumber { get; set; }
        public string? IssuedTo { get; set; }
    }
    public class BTODeliveryOrderSearchDto
    {
        public List<string> BTONumber { get; set; }
        public List<string> CustomerName { get; set; }
        public List<string> SalesOrderNumber { get; set; }
        public List<string> PONumber { get; set; }
        public List<String>? IssuedTo { get; set; }
    }
}
