using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCConfirmationItemsDto
    {
        public int? Id { get; set; }

        public string? PONumber { get; set; }

        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public string? MftrItemNumber { get; set; }
        public List<ProjectNumbers> ProjectNumber { get; set; }
        public string? ManufactureBatchNumber { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal? POOrderedQty { get; set; }

        [Precision(18, 3)]
        public decimal? POBalancedQty { get; set; }

        [Precision(18, 3)]
        public decimal? POUnitPrice { get; set; }
        public string? UOM { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? ReceivedQty { get; set; }
        public string? AcceptedQty { get; set; }
        public string? RejectedQty { get; set; }

    }

    public class IQCConfirmationItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        public string? ReceivedQty { get; set; }
        public string? AcceptedQty { get; set; }
        public string? RejectedQty { get; set; }
    }
    public class IQCConfirmationItemsUpdateDto
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        public string? ReceivedQty { get; set; }
        public string? AcceptedQty { get; set; }
        public string? RejectedQty { get; set; }
    }

}
