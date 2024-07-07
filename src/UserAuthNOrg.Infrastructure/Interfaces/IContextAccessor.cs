namespace UserAuthNOrg.Infrastructure.Interfaces
{
    public interface IContextAccessor
    {
        string GetCurrentUserId();

        string GetCurrentUserEmail();
    }
}
