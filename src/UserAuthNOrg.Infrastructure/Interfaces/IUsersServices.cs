using UserAuthNOrg.Core.ViewModel;
using UserAuthNOrg.Utilities.Extensions;

namespace UserAuthNOrg.Infrastructure.Interfaces
{
    public interface IUsersServices
    {
        Task<ApiResponse<ViewUser>> GetUser(string userId);
    }
}
