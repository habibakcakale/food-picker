namespace Meal.Controllers {
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Configuration;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Data;


    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory factory;
        private readonly FoodDbContext dbContext;

        public AccountController(IConfiguration configuration, IHttpClientFactory factory, FoodDbContext dbContext) {
            this.configuration = configuration;
            this.factory = factory;
            this.dbContext = dbContext;
        }

        [HttpGet("login")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = int.MinValue)]
        public async Task<IActionResult> Login() {
            if (User.Identity?.IsAuthenticated == true)
                return Redirect("/");
            await HttpContext.SignOutAsync();
            return Challenge();
        }

        [HttpGet("login-callback")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = int.MinValue)]
        public async Task<IActionResult> GoogleResponse(string code) {
            var token = await ExchangeCodeAsync(code, Url.ActionLink("GoogleResponse", "Account"));
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await factory.CreateClient().SendAsync(request);
            response.EnsureSuccessStatusCode();
            using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            var userInfo = MapUserInfo(payload.RootElement);
            await PersistUser(userInfo);

            var authToken = GenerateJwtToken(userInfo);
            return Content($"<script>opener.getToken({JsonSerializer.Serialize(authToken)}); close()</script>", "text/html");
        }

        private async Task PersistUser(User userInfo) {
            if (!await dbContext.Users.AnyAsync(item => item.Id == userInfo.Id)) {
                dbContext.Users.Add(userInfo);
                await dbContext.SaveChangesAsync();
            }
        }

        private User MapUserInfo(JsonElement payload) {
            var user = new User {
                Id = payload.GetString("id") ?? throw new ArgumentNullException(),
                Name = payload.GetString("name") ?? string.Empty,
                FirstName = payload.GetString("given_name") ?? string.Empty,
                LastName = payload.GetString("family_name") ?? string.Empty,
                Email = payload.GetString("email") ?? string.Empty,
                Picture = payload.GetString("picture") ?? string.Empty,
                SlackId = string.Empty
            };
            return user;
        }

        private string GenerateJwtToken(User user) {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("GOOGLE:ClientId"));
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                    new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                    new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                    new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(IdentityExtensions.PictureClaimType, user.Picture ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Authorize, HttpGet]
        public IActionResult UserInfo() {
            var userInfo = User.ToUser();

            return Ok(userInfo);
        }

        [HttpGet(nameof(BuildChallengeUrl))]
        public IActionResult BuildChallengeUrl() {
            // Google Identity Platform Manual:
            // https://developers.google.com/identity/protocols/OAuth2WebServer

            var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {"response_type", "code"},
                {"client_id", configuration.GetValue<string>("GOOGLE:ClientId")},
                {"redirect_uri", Url.ActionLink("GoogleResponse", "Account")},
                {"scope", "openid profile email"}
            };
            var url = QueryHelpers.AddQueryString("https://accounts.google.com/o/oauth2/v2/auth", queryStrings);
            return Ok(new {url});
        }

        protected virtual async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri) {
            var tokenRequestParameters = new Dictionary<string, string>() {
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
            if (!response.IsSuccessStatusCode) {
                throw new Exception("Invalid Code Response");
            }

            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return OAuthTokenResponse.Success(payload);
        }
    }

    public static class IdentityExtensions {
        public const string PictureClaimType = "picture";

        public static User ToUser(this ClaimsPrincipal principal) {
            var userInfo = new User() {Name = principal.Identity?.Name ?? string.Empty};
            foreach (var claim in principal.Claims) {
                switch (claim.Type) {
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
                    case PictureClaimType:
                        userInfo.Picture = claim.Value;
                        break;
                }
            }

            return userInfo;
        }

        public static string GetId(this ClaimsPrincipal identity) =>
            identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
