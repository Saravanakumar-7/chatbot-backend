using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
   public class CompanyApproval
    {
        [Key]
        public int Id { get; set; }
        public string? ScopeOfSupply { get; set; }

        public string? CompanyCategory { get; set; }

        public string? BasisOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;
        public string? CustomerApprove { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public List<CompanyFileUpload>? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
        public int CompanyMasterId { get; set; }

        public CompanyMaster? CompanyMaster { get; set; }
    }
}
