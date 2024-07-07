using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserAuthNOrg.Core.Context;
using UserAuthNOrg.Core.Models;
using UserAuthNOrg.Core.ViewModel;
using UserAuthNOrg.Infrastructure.Interfaces;
using UserAuthNOrg.Utilities.CoreConstants;
using UserAuthNOrg.Utilities.Extensions;

namespace UserAuthNOrg.Infrastructure.Services
{
    public class OrganizationServices : IOrganizationServices
    {
        private readonly ILogger<OrganizationServices> _logger;
        private readonly IContextAccessor _contextAccessor;
        private readonly UserAuthNOrgDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrganizationServices(
            ILogger<OrganizationServices> logger,
            IContextAccessor contextAccessor,
            UserAuthNOrgDbContext context)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _context = context;
        }

        public async Task<ApiResponse<ViewOrganizations>> GetOrganizations()
        {
            try
            {
                var userId = _contextAccessor.GetCurrentUserId();

                var organizations = await _context.UserOrganization
                    .Where(u => u.Id == userId)
                    .Include(o => o.Organizations)
                    .Select(s => new ViewOrganization()
                    {
                        OrgId = s.OrgId,
                        Name = s.Organizations.Name,
                        Description = s.Organizations.Description
                    }).ToListAsync();
                
                return new ApiResponse<ViewOrganizations>(
                    new ViewOrganizations()
                    {
                        Organizations = organizations
                    }, ConstantsString.Success);

            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong. Error --> {error}; Action Method : {method}; Action Path : {path};",
                                ex,
                                MethodClassForLogging.MethodName(),
                                MethodClassForLogging.MethodPath());

                return new ApiResponse<ViewOrganizations>(ConstantsString.Error, Utilities.Enums.StatusCode.ERROR);
            }
        }

        public async Task<ApiResponse<ViewOrganization>> GetOrganization(Guid orgId)
        {
            try
            {
                var userId = _contextAccessor.GetCurrentUserId();

                var organization = await _context.UserOrganization
                    .Include(o => o.Organizations)
                    .Where(u => u.OrgId == orgId && u.Id == userId)
                    .Select(s => new ViewOrganization()
                    {
                        OrgId = s.OrgId,
                        Name = s.Organizations.Name,
                        Description = s.Organizations.Description
                    }).FirstOrDefaultAsync();

                if (organization is null)
                    return new ApiResponse<ViewOrganization>(ConstantsString.NotFound, Utilities.Enums.StatusCode.NotFound);

                return new ApiResponse<ViewOrganization>(organization, ConstantsString.Success);

            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong. Error --> {error}; Action Method : {method}; Action Path : {path};",
                                ex,
                                MethodClassForLogging.MethodName(),
                                MethodClassForLogging.MethodPath());

                return new ApiResponse<ViewOrganization>(ConstantsString.Error, Utilities.Enums.StatusCode.ERROR);
            }
        }

        public async Task<ApiResponse<ViewOrganization>> CreateOrganization(CreateOrganizationDTO model)
        {
            try
            {
                var organization = new Organization()
                {
                    Name = model.Name,
                    Description = model.Description
                };

                await _context.Organizations.AddAsync(organization);

                var userId = _contextAccessor.GetCurrentUserId();

                var userOrganization = new UserOrganization()
                {
                    Id = userId,
                    OrgId = organization.OrgId
                };

                await _context.UserOrganization.AddAsync(userOrganization);
                await _context.SaveChangesAsync();

                return new ApiResponse<ViewOrganization>(
                    new ViewOrganization() 
                    { 
                        OrgId = organization.OrgId,
                        Name = organization.Name,
                        Description = organization.Description
                    }, "Organization created successfully", 
                    Utilities.Enums.StatusCode.Created);

            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong. Error --> {error}; Action Method : {method}; Action Path : {path};",
                                ex,
                                MethodClassForLogging.MethodName(),
                                MethodClassForLogging.MethodPath());

                return new ApiResponse<ViewOrganization>(ConstantsString.Error, Utilities.Enums.StatusCode.ERROR);
            }
        }

        public async Task<ApiResponse<string>> UpdateUsersOrganizations(Guid orgId, ViewUserDto model)
        {
            try
            {
                var org = await _context.UserOrganization.Where(o => o.Id == model.Userid && o.OrgId == orgId).FirstOrDefaultAsync();

                if (org is not null)
                    return new ApiResponse<string>(ConstantsString.UserExist, Utilities.Enums.StatusCode.BadRequest);

                var userOrg = new UserOrganization()
                {
                    OrgId = orgId,
                    Id = model.Userid
                };

                await _context.AddAsync(userOrg);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>(ConstantsString.UserAdded, Utilities.Enums.StatusCode.OK);

            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong. Error --> {error}; Action Method : {method}; Action Path : {path};",
                                ex,
                                MethodClassForLogging.MethodName(),
                                MethodClassForLogging.MethodPath());

                return new ApiResponse<string>(ConstantsString.Error, Utilities.Enums.StatusCode.ERROR);
            }
        }
    }
}
