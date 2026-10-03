using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinPath.Application.Users.Login
{
    public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResult>;
}
