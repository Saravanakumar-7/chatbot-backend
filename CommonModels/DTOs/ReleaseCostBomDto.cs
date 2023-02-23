using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReleaseCostBomDtoPost
    {
        public string ReleaseFor { get; set; }
        public string ItemNumber { get; set; }
        public string ReleaseVersion { get; set; }
        public string ReleaseNote { get; set; }
     
    }
}
