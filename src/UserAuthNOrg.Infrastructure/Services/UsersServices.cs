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
    public class UsersServices : IUsersServices
    {
        private readonly ILogger<UsersServices> _logger;
        private readonly UserManager<User> _userManager; 
        private readonly IContextAccessor _contextAccessor;
        private readonly UserAuthNOrgDbContext _context;

        public UsersServices(
            ILogger<UsersServices> logger,
            UserManager<User> userManager,
            IContextAccessor contextAccessor,
            UserAuthNOrgDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _context = context;
        }

        public async Task<ApiResponse<ViewUser>> GetUser(string userId)
        {
            try
            {
                User user;
                var userIdDb = _contextAccessor.GetCurrentUserId();

                if (userId == userIdDb)
                {
                    user = await _userManager.FindByIdAsync(userIdDb);

                    return new ApiResponse<ViewUser>(
                        new ViewUser()
                        {
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Phone = user.PhoneNumber,
                            UserId = userId
                        }, ConstantsString.Success);
                }

                var orgIds = await _context.UserOrganization
                    .Where(x => x.Id == userIdDb)
                    .Select(x => x.OrgId).ToListAsync();

                var userResponse = await _context.UserOrganization
                    .Include(u => u.Users)
                    .Where(x => x.Id == userId && orgIds.Contains(x.OrgId))
                    .FirstOrDefaultAsync();

                if (userResponse is null)
                    return new ApiResponse<ViewUser>(ConstantsString.NotFound, Utilities.Enums.StatusCode.NotFound);

                return new ApiResponse<ViewUser>(
                    new ViewUser()
                    {
                        Email = userResponse.Users.Email,
                        FirstName = userResponse.Users.FirstName,
                        LastName = userResponse.Users.LastName,
                        Phone = userResponse.Users.PhoneNumber,
                        UserId = userId
                    }, ConstantsString.Success);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Something went wrong. Error --> {error}; Action Method : {method}; Action Path : {path};",
                                ex,
                                MethodClassForLogging.MethodName(),
                                MethodClassForLogging.MethodPath());

                return new ApiResponse<ViewUser>(ConstantsString.Error, Utilities.Enums.StatusCode.ERROR);
            }
        }
    }
}
