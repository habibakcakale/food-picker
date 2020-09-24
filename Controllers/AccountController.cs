using System.Security.Claims;
using Meal.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return User.Identity.IsAuthenticated ? Redirect("/") : (IActionResult) Challenge(GoogleDefaults.AuthenticationScheme);
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
    }

    public static class IdentityExtensions
    {
        public static string GetId(this ClaimsPrincipal identity) => identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}