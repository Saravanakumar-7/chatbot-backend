using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EnggBom
    {
        public int Id { get; set; }

        public string ItemNumber { get; set; }

        public string? ItemDescription { get; set; }

        public string? ItemType { get; set; }

        public string? RevisionNumber { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<EnggChildItem>? EnggChildItems { get; set; }


    }
}
