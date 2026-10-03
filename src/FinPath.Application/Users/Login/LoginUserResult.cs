using System;
using System.Collections.Generic;
using System.Text;

namespace FinPath.Application.Users.Login
{
    public sealed record LoginUserResult(string AccessToken, DateTimeOffset ExpiresAtUtc);
}
