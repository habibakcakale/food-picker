using System.Collections.Generic;

namespace FoodApp.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
                return new ChallengeResult(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
                {
                    RedirectUri = Url.ActionLink()
                });
            return Redirect("/");
        }

        [Authorize]
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
    public class UserInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}