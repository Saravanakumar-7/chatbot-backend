using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class MaterialRequestSPReport
    {
        public string? MRNumber { get; set; }
        public string? ProjectType { get; set; }
        public PartType? ItemType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public decimal? indent_qnty { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? UOM { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
