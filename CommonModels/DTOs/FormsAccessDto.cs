using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class FormsAccessDto
    {
        public int Id { get; set; }
        public string? Menu { get; set; }
        public string? FormName { get; set; }

        [DefaultValue(false)]
        public bool Create { get; set; }

        [DefaultValue(false)]
        public bool Delete { get; set; }

        [DefaultValue(false)]
        public bool View { get; set; }

        [DefaultValue(false)]
        public bool Edit { get; set; }

        [DefaultValue(false)]
        public bool Download { get; set; }

        [DefaultValue(false)]
        public bool ApprovalI { get; set; }

        [DefaultValue(false)]
        public bool ApprovalII { get; set; }
        [DefaultValue(false)]
        public bool Print { get; set; }
    }
}
