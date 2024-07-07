namespace UserAuthNOrg.Core.ViewModel
{
    public record ViewOrganization
    {
        public Guid OrgId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public record ViewOrganizations
    {
        public List<ViewOrganization> Organizations { get; set; } = new();
    }

    public record ViewUserDto
    {
        public string Userid { get; set; }
    }
}
