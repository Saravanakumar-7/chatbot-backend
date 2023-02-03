using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnDeliveryOrderItemsDto
    {
        public int Id { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 2)]
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 2)]
        public decimal? FGOrderQty { get; set; }

        [Precision(13, 2)]
        public decimal? OrderBalanceQty { get; set; }
        public int? FGStock { get; set; }

        [Precision(13, 2)]
        public decimal? DispatchQty { get; set; }

        [Precision(13, 2)]
        public decimal? AlreadyReturnQty { get; set; }

        [Precision(13, 2)]
        public decimal? ReturnQty { get; set; }

        [Precision(13, 2)]
        public decimal? BalanceQty { get; set; }

        public string? Remarks { get; set; }
        public string? SerialNo { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }


    }

    public class ReturnDeliveryOrderItemsDtoPost
    {
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 2)]
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public int SalesOrderId { get; set; }

        [Precision(13, 2)]
        public decimal? FGOrderQty { get; set; }

        [Precision(13, 2)]
        public decimal? OrderBalanceQty { get; set; }
        public int? FGStock { get; set; }

        [Precision(13, 2)]
        public decimal? DispatchQty { get; set; }

        [Precision(13, 2)]
        public decimal? AlreadyReturnQty { get; set; }

        [Precision(13, 2)]
        public decimal? ReturnQty { get; set; }

        [Precision(13, 2)]
        public decimal? BalanceQty { get; set; }

        public string? Remarks { get; set; }
        public string? SerialNo { get; set; }


    }

    public class ReturnDeliveryOrderItemsDtoUpdate
    {
        public int? Id { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 2)]
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 2)]
        public decimal? FGOrderQty { get; set; }

        [Precision(13, 2)]
        public decimal? OrderBalanceQty { get; set; }
        public int? FGStock { get; set; }

        [Precision(13, 2)]
        public decimal? DispatchQty { get; set; }

        [Precision(13, 2)]
        public decimal? AlreadyReturnQty { get; set; }

        [Precision(13, 2)]
        public decimal? ReturnQty { get; set; }

        [Precision(13, 2)]
        public decimal? BalanceQty { get; set; }

        public string? Remarks { get; set; }
        public string? SerialNo { get; set; }


        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class BtoDOReturnQtyDetailsDto
    {

        public string FGPartNumber { get; set; }
        public int SalesOrderId { get; set; }
        public decimal ReturnQty { get; set; }

    }

}
