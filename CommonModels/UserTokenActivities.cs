using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserTokenActivities
    {
        [Key]
        public int RegistrationId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get;set; }
        public string Token { get; set; }
        public DateTime Validity { get; set; }
        public bool TokenIsActive { get; set; }
    }
}
