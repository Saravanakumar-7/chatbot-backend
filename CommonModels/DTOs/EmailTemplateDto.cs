using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EmailTemplateDto
    {
        public int Id { get; set; }
        public string? ProcessType { get; set; }
        public string? Template { get; set; }
        public string? Subject { get; set; }
    }
}
