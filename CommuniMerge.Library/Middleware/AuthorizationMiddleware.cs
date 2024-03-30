using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using CommuniMerge.Library.Middleware.Objects;
using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CommuniMerge.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthorizationMiddleware> logger;
        private readonly TokenSettings tokenSettings;

        public AuthorizationMiddleware(RequestDelegate next, IOptions<TokenSettings> tokenSettings, ILogger<AuthorizationMiddleware> logger)
        {
            this._next = next;
            this.logger = logger;
            this.tokenSettings = tokenSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["BearerToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            if (!string.IsNullOrEmpty(token))
            {
                var decodedToken = DecodeToken(token);

                if (decodedToken != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, decodedToken.UserId),
                        new Claim(ClaimTypes.Name, decodedToken.Username)
                    };

                    var identity = new ClaimsIdentity(claims, "Bearer");
                    context.User = new ClaimsPrincipal(identity);
                }
            }

            await _next(context);
        }

        private TokenPayload DecodeToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(tokenSettings.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                SecurityToken validatedToken;

                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;

                return new TokenPayload
                {
                    UserId = jwtToken.Subject,
                    Username = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.UniqueName)?.Value
                };
            }
            catch(SecurityTokenValidationException stve)
            {
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError($"[{DateTime.UtcNow}] AuthorizationMiddleware - decode token: {ex}");
                return null;
            }
        }
    }
}
