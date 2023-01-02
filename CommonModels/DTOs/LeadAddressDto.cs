using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class LeadAddressDto
    {
        public int Id { get; set; }

        public string? ProjectName { get; set; }

        public string? Addresses { get; set; }

        public string? Villa_House_Flat { get; set; }

        public string? Street { get; set; }

        public string? Area { get; set; }

        public string? Zone { get; set; }

        public string? LandMark { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }
        public string? ZIP { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class LeadAddressPostDto
    {
        public string? ProjectName { get; set; }

        [StringLength(500, ErrorMessage = "Address can't be longer than 500 characters")]
        public string? Addresses { get; set; }

        public string? Villa_House_Flat { get; set; }

        [StringLength(200, ErrorMessage = "Street can't be longer than 200 characters")]
        public string? Street { get; set; }

        [StringLength(500, ErrorMessage = "Area can't be longer than 500 characters")]
        public string? Area { get; set; }

        [StringLength(100, ErrorMessage = "Zone can't be longer than 100 characters")]
        public string? Zone { get; set; }

        [StringLength(100, ErrorMessage = "LandMark can't be longer than 100 characters")]
        public string? LandMark { get; set; }

        [StringLength(100, ErrorMessage = "City can't be longer than 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "State can't be longer than 100 characters")]
        public string? State { get; set; }

        [StringLength(100, ErrorMessage = "Country can't be longer than 100 characters")]
        public string? Country { get; set; }

        [StringLength(100, ErrorMessage = "ZIP can't be longer than 100 characters")]
        public string? ZIP { get; set; }

    }

    public class LeadAddressUpdateDto
    {
        public int Id { get; set; }
        public string? ProjectName { get; set; }

        [StringLength(500, ErrorMessage = "Address can't be longer than 500 characters")]
        public string? Addresses { get; set; }

        public string? Villa_House_Flat { get; set; }

        [StringLength(200, ErrorMessage = "Street can't be longer than 200 characters")]
        public string? Street { get; set; }

        [StringLength(500, ErrorMessage = "Area can't be longer than 500 characters")]
        public string? Area { get; set; }

        [StringLength(100, ErrorMessage = "Zone can't be longer than 100 characters")]
        public string? Zone { get; set; }

        [StringLength(100, ErrorMessage = "LandMark can't be longer than 100 characters")]
        public string? LandMark { get; set; }

        [StringLength(100, ErrorMessage = "City can't be longer than 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "State can't be longer than 100 characters")]
        public string? State { get; set; }

        [StringLength(100, ErrorMessage = "Country can't be longer than 100 characters")]
        public string? Country { get; set; }

        [StringLength(100, ErrorMessage = "ZIP can't be longer than 100 characters")]
        public string? ZIP { get; set; }
        public string Unit { get; set; }
    }

}
