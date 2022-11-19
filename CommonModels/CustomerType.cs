using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class CustomerType
    {
        [Key]
        public int? Id { get; set; }
        public string? CustomerTypeName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}