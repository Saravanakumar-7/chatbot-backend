using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string UnitName { get; set; }
    }
    public class LoginResponseDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string UnitName { get; set; }

    }
}
