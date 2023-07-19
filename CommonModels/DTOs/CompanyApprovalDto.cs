using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CompanyApprovalDto
    {
        public int Id { get; set; }
        public string? ScopeOfSupply { get; set; }

        public string? CompanyCategory { get; set; }

        public string? BasisOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;
        public string? CustomerApprove { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public List<CompanyFileUploadDto>? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
    }
    public class CompanyApprovalPostDto
    {
        public string? ScopeOfSupply { get; set; }

        public string? CompanyCategory { get; set; }

        public string? BasisOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;
        public string? CustomerApprove { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public List<CompanyFileUploadPostDto>? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
    }
    public class CompanyApprovalUpdateDto
    {
        public int Id { get; set; }
        public string? ScopeOfSupply { get; set; }

        public string? CompanyCategory { get; set; }

        public string? BasisOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;
        public string? CustomerApprove { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public List<CompanyFileUploadUpdateDto>? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
    }
}
