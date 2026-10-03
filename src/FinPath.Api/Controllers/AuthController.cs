using FinPath.Api.Contracts.Authentication.CurrentUser;
using FinPath.Api.Contracts.Authentication.Login;
using FinPath.Api.Contracts.Authentication.Register;
using FinPath.Application.Common.Exceptions;
using FinPath.Application.Users.Login;
using FinPath.Application.Users.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinPath.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponse>> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(
                    request.Email,
                    request.DisplayName,
                    request.Password
                );

            var result = await _sender.Send(command, cancellationToken);

            var response = new RegisterUserResponse
            {
                Id = result.Id,
                Email = result.Email,
                DisplayName = result.DisplayName
            };

            return Created($"/api/v1/users/{result.Id}", response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(
                    request.Email,
                    request.Password
                );

            var result = await _sender.Send(command, cancellationToken);

            var response = new LoginUserResponse
            {
                AccessToken = result.AccessToken,
                ExpiresAtUtc = result.ExpiresAtUtc
            };

            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<CurrentUserResponse> GetCurrentUser()
        {
            var userIdValue = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userIdValue is null || !Guid.TryParse(userIdValue, out var userId))
                throw new UnauthorizedException("Токен не содержит корректный идентификатор пользователя.");

            var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedException("Токен не содержит адрес электронной почты пользователя.");

            var name = User.FindFirstValue(JwtRegisteredClaimNames.Name);
            if (string.IsNullOrWhiteSpace(name))
                throw new UnauthorizedException("Токен не содержит отображаемое имя пользователя.");

            var response = new CurrentUserResponse
            {
                Id = userId,
                Email = email,
                DisplayName = name
            };

            return Ok(response);
        }
    }
}
