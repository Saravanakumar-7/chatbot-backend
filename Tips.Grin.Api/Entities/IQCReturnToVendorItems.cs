using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class IQCReturnToVendorItems
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string PONumber { get; set; }
        [Precision(13, 3)]
        public decimal InitialRejectQty { get; set; }
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        public string? Remarks { get; set; }    
        public int IQCReturnToVendorId { get; set; }
        public IQCReturnToVendor? IQCReturnToVendor { get; set; }
    }
}
