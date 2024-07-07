using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAuthNOrg.Core.Models
{
    public class UserOrganization : BaseEntity
    {
        [Key]
        public Guid UserOrgId { get; set; }

        public string Id { get; set; }
        
        [ForeignKey(nameof(Id))]
        public virtual User Users { get; set; }

        public Guid OrgId { get; set; }

        [ForeignKey(nameof(OrgId))]
        public virtual Organization Organizations { get; set; }
    }
}
