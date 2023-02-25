using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReleaseCostBomDtoPost
    {
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public decimal ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }

    }
    public class GetAllReleaseCostBomItemNumberVersionList
    {
        public string ItemNumber { set; get; }
        public decimal[] ReleaseVersion { get; set; }

    }
}
