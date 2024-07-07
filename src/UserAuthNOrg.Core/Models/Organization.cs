using System.ComponentModel.DataAnnotations;

namespace UserAuthNOrg.Core.Models
{
    public class Organization : BaseEntity
    {
        [Key]
        public Guid OrgId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<UserOrganization> UsersOrganizations { get; set; } = new List<UserOrganization>();
    }
}
