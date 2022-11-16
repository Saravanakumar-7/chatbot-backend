using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class HeadCounting
    {
        public int Id { get; set; }

        public string? DepartmentSkill { get; set; }

        public string? NumberOfPeople { get; set; }

        public int VendorMasterId { get; set; }

        public VendorMaster? VendorMaster { get; set; }

    }
}
