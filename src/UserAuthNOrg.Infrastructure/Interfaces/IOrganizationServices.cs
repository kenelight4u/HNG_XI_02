using UserAuthNOrg.Core.ViewModel;
using UserAuthNOrg.Utilities.Extensions;

namespace UserAuthNOrg.Infrastructure.Interfaces
{
    public interface IOrganizationServices
    {
        Task<ApiResponse<ViewOrganizations>> GetOrganizations();

        Task<ApiResponse<ViewOrganization>> GetOrganization(Guid orgId);

        Task<ApiResponse<ViewOrganization>> CreateOrganization(CreateOrganizationDTO model);

        Task<ApiResponse<string>> UpdateUsersOrganizations(Guid orgId, ViewUserDto model);
    }
}
