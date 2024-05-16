using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EmailIDsDto
    {
        public int Id { get; set; }
        public string Operation { get; set; }
        public string EmailIds { get; set; }
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? Password { get; set; }
    }
}
