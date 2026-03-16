using Eccomerce_Web.CORE;
using Eccomerce_Web.Models.Admin;
using Eccomerce_Web.Models.User;

namespace Eccomerce_Web.Common.Services.Interfaces
{
    public interface IJWTService
    {
        UserToken GetUserToken(UserProfile user);

        AdminToken GetAdminToken(Admin admin);
    }
}
