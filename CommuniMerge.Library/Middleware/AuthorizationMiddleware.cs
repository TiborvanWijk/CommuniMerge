using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using CommuniMerge.Library.Middleware.Objects;
using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CommuniMerge.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenSettings _tokenSettings;

        public AuthorizationMiddleware(RequestDelegate next, IOptions<TokenSettings> tokenSettings)
        {
            _next = next;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["BearerToken"];

            var allcookies = context.Request.Cookies;

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
                    context.User.AddIdentity(identity);
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
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_tokenSettings.SecretKey)),
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
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
