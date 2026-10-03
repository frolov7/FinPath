using FinPath.Application.Common.Authentication;
using FinPath.Domain.Users;

namespace FinPath.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        AccessToken Generate(User user);
    }
}
