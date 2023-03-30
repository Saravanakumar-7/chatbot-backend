using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class RegistrationFormDto
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Compare("PasswordConfirm")]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RegistrationFormPostDto
    {

        [Required]
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Compare("PasswordConfirm")]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }

    }
    public class RegistrationFormUpdateDto
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Compare("PasswordConfirm")]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }
        public string Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RegistrationFormDetailsDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}