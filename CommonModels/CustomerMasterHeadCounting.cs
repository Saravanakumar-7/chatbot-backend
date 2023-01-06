using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CustomerMasterHeadCounting
    {
        public int Id { get; set; }
        public string? DepartmentSkill { get; set; }

        public string? NumberOfPeople { get; set; }


        public int CustomerMasterId { get; set; }

        public CustomerMaster? CustomerMaster { get; set; }
    }
}
