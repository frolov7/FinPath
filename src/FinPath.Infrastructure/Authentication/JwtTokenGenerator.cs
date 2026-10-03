using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinPath.Application.Common.Authentication;
using FinPath.Application.Common.Interfaces;
using FinPath.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinPath.Infrastructure.Authentication
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;

        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            ArgumentNullException.ThrowIfNull(jwtOptions);

            _jwtOptions = jwtOptions.Value;
        }

        public AccessToken Generate(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var issuedAtUtc = DateTimeOffset.UtcNow;

            var expiresAtUtc = issuedAtUtc.AddMinutes(
                _jwtOptions.ExpirationMinutes);

            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email),

                new Claim(
                    JwtRegisteredClaimNames.Name,
                    user.DisplayName),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    issuedAtUtc
                        .ToUnixTimeSeconds()
                        .ToString(CultureInfo.InvariantCulture),
                    ClaimValueTypes.Integer64)
            };

            var secretBytes = Convert.FromBase64String(
                _jwtOptions.Secret);

            var signingKey = new SymmetricSecurityKey(
                secretBytes);

            var signingCredentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: issuedAtUtc.UtcDateTime,
                expires: expiresAtUtc.UtcDateTime,
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValue = tokenHandler.WriteToken(jwt);

            return new AccessToken(
                tokenValue,
                expiresAtUtc);
        }
    }
}