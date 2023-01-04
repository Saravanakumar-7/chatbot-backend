using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tips.Production.Api.Entities.DTOs
{
    public class SAShopOrderDto
    {
        
        public int Id { get; set; }

        
        public string? SAShopOrderNumber { get; set; }


        public string? ProjectType { get; set; }


        public string? ProjectNumber { get; set; }


        public string? FGItemNumber { get; set; }


        public string? SAItemNumber { get; set; }


        public string? Description { get; set; }


        public string? SalesOrderNumber { get; set; }


        public decimal SalesOrderQty { get; set; }


        public decimal SAShopOrderReleaseQty { get; set; }


        public DateTime? SAShopOrderCloseDate { get; set; }


        public string? SalesOrderPONumber { get; set; }


        public OrderStatus Status { get; set; }


        public decimal WipQty { get; set; }


        public decimal OqcQty { get; set; }


        public decimal ScrapQty { get; set; }


        public bool IsDeleted { get; set; }


        public bool IsShortClosed { get; set; }

        public DateTime? ShortClosedOn { get; set; }


        public string? ShorClosedBy { get; set; }


        public IssueStatus MaterialIssueStatus { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public string? Unit { get; set; }

    }

    public class SAShopOrderDtoPost
    {
        
        public string? SAShopOrderNumber { get; set; }


        public string? ProjectType { get; set; }


        public string? ProjectNumber { get; set; }


        public string? FGItemNumber { get; set; }


        public string? SAItemNumber { get; set; }


        public string? Description { get; set; }


        public string? SalesOrderNumber { get; set; }


        public decimal SalesOrderQty { get; set; }


        public decimal SAShopOrderReleaseQty { get; set; }


        public DateTime? SAShopOrderCloseDate { get; set; }


        public string? SalesOrderPONumber { get; set; }


        public OrderStatus Status { get; set; }


        public decimal WipQty { get; set; }


        public decimal OqcQty { get; set; }


        public decimal ScrapQty { get; set; }


        public bool IsDeleted { get; set; }


        public bool IsShortClosed { get; set; }

        public DateTime? ShortClosedOn { get; set; }


        public string? ShorClosedBy { get; set; }


        public IssueStatus MaterialIssueStatus { get; set; }

        

    }

    public class SAShopOrderDtoUpdate
    {
        public int Id { get; set; }


        public string? SAShopOrderNumber { get; set; }


        public string? ProjectType { get; set; }


        public string? ProjectNumber { get; set; }


        public string? FGItemNumber { get; set; }


        public string? SAItemNumber { get; set; }


        public string? Description { get; set; }


        public string? SalesOrderNumber { get; set; }


        public decimal SalesOrderQty { get; set; }


        public decimal SAShopOrderReleaseQty { get; set; }


        public DateTime? SAShopOrderCloseDate { get; set; }


        public string? SalesOrderPONumber { get; set; }


        public OrderStatus Status { get; set; }


        public decimal WipQty { get; set; }


        public decimal OqcQty { get; set; }


        public decimal ScrapQty { get; set; }


        public bool IsDeleted { get; set; }


        public bool IsShortClosed { get; set; }

        public DateTime? ShortClosedOn { get; set; }


        public string? ShorClosedBy { get; set; }


        public IssueStatus MaterialIssueStatus { get; set; }

        

    }


}
