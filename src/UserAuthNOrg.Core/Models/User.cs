using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UserAuthNOrg.Core.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public override string Email { get; set; }

        public string Salt { get; set; }

        public DateTime? PasswordResetDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<UserOrganization> UsersOrganizations { get; set; } = new List<UserOrganization>();
    }
}
