using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCReturnToVendorItemsPostDto
    {
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public string PONumber { get; set; }
        public int GrinPartId { get; set; }
        [Precision(13, 3)]
        public decimal InitialRejectQty { get; set; }
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class IQCReturnToVendorItemsDto
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public string PONumber { get; set; }
        public decimal InitialRejectQty { get; set; }
        public decimal ReturnQty { get; set; }
        public string? Remarks { get; set; }
    }
}
