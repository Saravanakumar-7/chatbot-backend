using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CustomerCategory
    {
        public int Id { get; set; }
        public string? CustomerCategoryName { get; set; }
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
