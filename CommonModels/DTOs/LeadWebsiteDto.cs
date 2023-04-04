using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class LeadWebsiteDto
    {
        public int Id { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PropertyType { get; set; }

        [Precision(13, 3)]

        public decimal? PropertySize { get; set; }

        public string? StageOfConstruction { get; set; }

        public bool? IsActive { get; set; } = true;

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class LeadWebsitePostDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PropertyType { get; set; }

        [Precision(13, 3)]

        public decimal? PropertySize { get; set; }

        public string? StageOfConstruction { get; set; }
        public bool? IsActive { get; set; } = true;

    }
    public class LeadWebsiteUpdateDto
    {
        public int Id { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PropertyType { get; set; }

        [Precision(13, 3)]

        public decimal? PropertySize { get; set; }

        public string? StageOfConstruction { get; set; }

        public bool? IsActive { get; set; } = true;

    }

}
