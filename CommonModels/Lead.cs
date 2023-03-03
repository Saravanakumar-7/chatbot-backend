using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Lead
    {
        [Key]
        public int Id { get; set; }

        public string? LeadID { get; set; }

        public string? ContactName { get; set; }

        public string? CustomerFirstName { get; set; }

        public string? CustomerLastName { get; set; }

        public string? CompanyName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Mobile { get; set; }

        public string? OptIn { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? GsheetSalesCount { get; set; }

        public DateTime? Anniversary { get; set; }

        public string? Occupation { get; set; }
        public string? GISLocation { get; set; }
        public string? LostReason { get; set; }

        public string? LostRemarks { get; set; }

        public string? TypeOfHome { get; set; }

        public string? BHK { get; set; }
        public string? SFT { get; set; }

        public string? Upgradeto { get; set; }

        public string? Opportunity { get; set; }

        public string? TypeOfSolution { get; set; }

        public string? DemoStatus { get; set; }

        public string? LeadStatus { get; set; }

        public string? CustomerType { get; set; }

        public string? LeadType { get; set; }

        public string? CustomerSegment { get; set; }
        public string? Salutation { get; set; }        

        public string? SecondarySource { get; set; }

        public string? Source { get; set; }

        public string? SourceDetails { get; set; }

        public DateTime? ExpectedClosing { get; set; }

        public string? ArchitectName { get; set; }

        public string? PMCContractor { get; set; }

        public string? PMCMobile { get; set; }

        public string? LightingDesigner { get; set; }

        public string? Observer { get; set; }

        public string? StagOfConstuction { get; set; }

        public string? Tags { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<LeadAddress>? LeadAddress { get; set; }

    }
}
