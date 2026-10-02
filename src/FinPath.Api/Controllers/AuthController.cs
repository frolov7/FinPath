using MediatR;
using Microsoft.AspNetCore.Mvc;
using FinPath.Api.Contracts.Authentication;
using FinPath.Application.Users.Register;
using System.ComponentModel.DataAnnotations;

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
    }
}
