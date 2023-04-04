using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class MaterialReturnNote
    {
        public int? Id { get; set; }
        public string? MRNNumber { get; set; }
        public string? ProjectNumber { get; set; } 
        public PartType ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialReturnNoteItem>? MaterialReturnNoteItems { get; set; }
    }
}
