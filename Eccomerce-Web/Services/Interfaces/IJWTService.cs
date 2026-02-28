using Eccomerce_Web.CORE;
using Eccomerce_Web.Models;

namespace Eccomerce_Web.Services.Interfaces
{
    public interface IJWTService
    {
        UserToken GetUserToken(UserProfile user);
    }
}
