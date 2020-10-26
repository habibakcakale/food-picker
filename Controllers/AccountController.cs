using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;

namespace Meal.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Configuration;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Meal.Models;

    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory factory;

        public AccountController(IConfiguration configuration, IHttpClientFactory factory)
        {
            this.configuration = configuration;
            this.factory = factory;
        }

        [HttpGet("login")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = int.MinValue)]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            await HttpContext.SignOutAsync();
            return Challenge();
        }

        [HttpGet("login-callback")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = int.MinValue)]
        public async Task<IActionResult> GoogleResponse(string code)
        {
            var token = await ExchangeCodeAsync(code, Url.ActionLink("GoogleResponse", "Account"));
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await factory.CreateClient().SendAsync(request);

            using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var authToken = GenerateJwtToken(payload);
            return Content($"<script>opener.getToken({JsonSerializer.Serialize(authToken)}); close()</script>", "text/html");
        }

        private string GenerateJwtToken(JsonDocument payload)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("GOOGLE:ClientId"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, payload.RootElement.GetString("id")),
                    new Claim(ClaimTypes.Name, payload.RootElement.GetString("name")),
                    new Claim(ClaimTypes.GivenName, payload.RootElement.GetString("given_name")),
                    new Claim(ClaimTypes.Surname, payload.RootElement.GetString("family_name")),
                    new Claim(ClaimTypes.Email, payload.RootElement.GetString("email")),
                    new Claim("picture", payload.RootElement.GetString("picture")), 
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Authorize, HttpGet]
        public IActionResult UserInfo()
        {
            var userInfo = new UserInfo {Name = User.Identity.Name ?? string.Empty};
            foreach (var claim in User.Claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.Email:
                        userInfo.Email = claim.Value;
                        break;
                    case ClaimTypes.NameIdentifier:
                        userInfo.Id = claim.Value;
                        break;
                    case ClaimTypes.GivenName:
                        userInfo.FirstName = claim.Value;
                        break;
                    case ClaimTypes.Surname:
                        userInfo.LastName = claim.Value;
                        break;
                }
            }

            return Ok(userInfo);
        }

        [HttpGet(nameof(BuildChallengeUrl))]
        public IActionResult BuildChallengeUrl()
        {
            // Google Identity Platform Manual:
            // https://developers.google.com/identity/protocols/OAuth2WebServer

            var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"response_type", "code"},
                {"client_id", configuration.GetValue<string>("GOOGLE:ClientId")},
                {"redirect_uri", Url.ActionLink("GoogleResponse", "Account")},
                {"scope", "openid profile email"}
            };
            var url = QueryHelpers.AddQueryString("https://accounts.google.com/o/oauth2/v2/auth", queryStrings);
            return Ok(new {url});
        }

        protected virtual async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                {"client_id", configuration.GetValue<string>("GOOGLE:ClientId")},
                {"redirect_uri", redirectUri},
                {"client_secret", configuration.GetValue<string>("GOOGLE:ClientSecret")},
                {"code", code},
                {"grant_type", "authorization_code"},
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.googleapis.com/oauth2/v4/token");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = requestContent;
            var response = await factory.CreateClient().SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return OAuthTokenResponse.Success(payload);
            }
            else
            {
                throw new Exception("Invalid Code Response");
            }
        }
    }

    public static class IdentityExtensions
    {
        public static string GetId(this ClaimsPrincipal identity) => identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}