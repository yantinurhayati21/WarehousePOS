using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WarehousePOS.Services
{
    public class ExternalTokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ExternalTokenAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token = null;
            
            if (Request.Headers.TryGetValue("token", out var tokenHeader))
            {
                token = tokenHeader.ToString();
            }
            else if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var authValue = authHeader.ToString();
                token = authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authValue.Substring(7).Trim()
                    : authValue.Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                Logger.LogWarning("Token tidak ditemukan atau kosong");
                return Task.FromResult(AuthenticateResult.Fail("Token required. Gunakan header 'token' atau 'Authorization: Bearer {token}'"));
            }

            Logger.LogInformation("Token valid, akses diberikan");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "ExternalUser"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("Token", token)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
