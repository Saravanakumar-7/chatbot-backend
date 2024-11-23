using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class IQCReturnToVendorItems
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? PONumber { get; set; }
        public int GrinPartId { get; set; }
        [Precision(13, 3)]
        public decimal InitialQty { get; set; }
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        public string? Remarks { get; set; }       
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<IQCReturnToVendorItemsProject> iQCReturnToVendorItemsProjects { get; set; }
        public int IQCReturnToVendorId { get; set; }
        public IQCReturnToVendor? IQCReturnToVendor { get; set; }
    }
}
